using M3U8.NET.Constants;
using M3U8.NET.Exceptions;
using M3U8.NET.Parsers.Interfaces;
using M3U8.NET.Parsing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace M3U8.NET.Parsers;

internal partial class M3U8Parser : IPlaylistParser
{
    public Playlist Parse(string content)
    {
        return Parse(content, options: new ParseOptions());
    }

    public Playlist Parse(string content, ParseOptions? options = null)
    {
        var opts = options ?? new ParseOptions();

        if (!TryParse(content, out var playlist, out var warnings, opts))
        {
            var firstError = warnings.FirstOrDefault(w => w.Level == WarningLevel.Error);
            if (firstError != null)
                throw new InvalidM3U8FormatException(
                    $"{firstError.Message} (Line {firstError.LineNumber}): {firstError.RawLine}");

            throw new InvalidM3U8FormatException("Failed to parse playlist (unknown error).");
        }

        foreach (var warning in warnings)
            opts.OnWarning?.Invoke(warning);

        return playlist!;
    }

    public bool TryParse(
        string content,
        out Playlist? playlist,
        out IReadOnlyList<ParseWarning> warnings,
        ParseOptions? options = null
    )
    {
        var opts = options ?? new ParseOptions();
        var warnList = new List<ParseWarning>();
        playlist = null;

        var sessionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var segments = new List<Segment>();
        int hlsVersion = 0;

        try
        {
            if (opts.RemoveBOM && content.Length > 0 && content[0] == '\uFEFF')
                content = content[1..];

            using var reader = new StringReader(content);

            string? line = reader.ReadLine();
            int lineNumber = 1;

            if (string.IsNullOrWhiteSpace(line) || line.Trim() != Tag.EXTM3U)
            {
                var warning = new ParseWarning(lineNumber, line ?? "", $"Playlist must start with {Tag.EXTM3U}.", WarningLevel.Error);
                warnList.Add(warning);
                opts.OnWarning?.Invoke(warning);
                warnings = warnList;
                return false;
            }

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (opts.TrimLines)
                    line = line.Trim();

                if (string.IsNullOrWhiteSpace(line) || char.IsControl(line[0]))
                    continue;

                try
                {
                    if (line.StartsWith(Tag.EXTXSESSIONDATA))
                    {
                        var dataPart = line[Tag.EXTXSESSIONDATA.Length..];
                        var attributes = ParseAttributes(dataPart);
                        foreach (var attr in attributes)
                            sessionData[attr.Key] = attr.Value.Trim('"');
                    }
                    else if (line.StartsWith(Tag.EXTINF, StringComparison.OrdinalIgnoreCase))
                    {
                        double duration = 0;
                        string? title = null;
                        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        var infPart = line[Tag.EXTINF.Length..];
                        var parts = infPart.Split([','], 2);

                        if (parts.Length >= 1)
                        {
                            var durationAndAttrs = parts[0].Trim();
                            var durationStr = durationAndAttrs.Split(' ')[0];
                            if (!double.TryParse(durationStr, out duration))
                            {
                                var w = new ParseWarning(lineNumber, line, $"Invalid duration format: {durationStr}", WarningLevel.Error);
                                warnList.Add(w);
                                opts.OnWarning?.Invoke(w);
                                continue;
                            }

                            var attrsStr = string.Join(" ", durationAndAttrs.Split(' ').Skip(1));
                            attributes = ParseAttributes(attrsStr);
                        }

                        if (parts.Length == 2)
                            title = parts[1].Trim();

                        var nextLine = reader.ReadLine()?.Trim();
                        lineNumber++;
                        if (!string.IsNullOrWhiteSpace(nextLine) && !nextLine.StartsWith("#"))
                        {
                            var segment = new Segment(duration, title, nextLine!, attributes);
                            segments.Add(segment);
                        }
                        else
                        {
                            var w = new ParseWarning(lineNumber, nextLine ?? "", "Missing URI after #EXTINF.", WarningLevel.Error);
                            warnList.Add(w);
                            opts.OnWarning?.Invoke(w);
                        }
                    }
                    else if (line.StartsWith(Tag.EXTXVERSION, StringComparison.OrdinalIgnoreCase))
                    {
                        var versionStr = line[Tag.EXTXVERSION.Length..].Trim();
                        if (int.TryParse(versionStr, out var version))
                            hlsVersion = version;
                    }
                    else if (line.StartsWith("#") && !opts.AllowNonStandardTags)
                    {
                        var w = new ParseWarning(lineNumber, line, $"Unknown tag not allowed: {line}", WarningLevel.Warning);
                        warnList.Add(w);
                        opts.OnWarning?.Invoke(w);
                    }
                }
                catch (Exception ex)
                {
                    var w = new ParseWarning(lineNumber, line, $"Exception while parsing line: {ex.Message}", WarningLevel.Error);
                    warnList.Add(w);
                    opts.OnWarning?.Invoke(w);
                }
            }

            playlist = new Playlist(
                hlsVersion,
                sessionData,
                segments
            );
        }
        catch (Exception ex)
        {
            warnList.Add(new ParseWarning(0, "", $"Unexpected parser exception: {ex.Message}", WarningLevel.Error));
            playlist = null;
            warnings = warnList;
            return false;
        }

        warnings = warnList;
        return !warnings.Any(w => w.Level == WarningLevel.Error);
    }

    public async IAsyncEnumerable<Segment> ParseSegmentsAsync(
        Stream input,
        ParseOptions? options = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var opts = options ?? new ParseOptions();
        using var reader = new StreamReader(input, detectEncodingFromByteOrderMarks: true);
        string? line;
        int lineNumber = 0;

        double duration = 0;
        string? title = null;
        Dictionary<string, string> attributes = [];

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            line = await reader.ReadLineAsync(ct).ConfigureAwait(false);
            if (line == null)
                break;
            lineNumber++;

            if (opts.TrimLines)
                line = line.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith(Tag.EXTINF, StringComparison.OrdinalIgnoreCase))
            {
                var infPart = line[Tag.EXTINF.Length..];
                var parts = infPart.Split([','], 2);

                if (parts.Length >= 1)
                {
                    var durationAndAttrs = parts[0].Trim();
                    var durationStr = durationAndAttrs.Split(' ')[0];
                    _ = double.TryParse(durationStr, out duration);
                    var attrsStr = string.Join(" ", durationAndAttrs.Split(' ').Skip(1));
                    attributes = ParseAttributes(attrsStr);
                }

                if (parts.Length == 2)
                    title = parts[1].Trim();

                string? nextLine = await reader.ReadLineAsync().ConfigureAwait(false);
                lineNumber++;
                if (!string.IsNullOrWhiteSpace(nextLine) && !nextLine.StartsWith("#"))
                {
                    yield return new Segment(duration, title, nextLine!, attributes);
                }
            }
        }
    }

    private static Dictionary<string, string> ParseAttributes(string attrString)
    {
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(attrString))
            return attributes;

        if (attrString.Length > 1024)
            throw new InvalidM3U8FormatException("Attributes too long.");

        var regex = KeyValueRegex();
        var matches = regex.Matches(attrString);

        foreach (Match match in matches)
        {
            if (match.Groups.Count >= 3)
            {
                var key = match.Groups[1].Value.Trim();
                var value = match.Groups[3].Value;
                if (string.IsNullOrEmpty(value))
                    value = match.Groups[4].Value;
                if (string.IsNullOrEmpty(value))
                    value = match.Groups[5].Value;

                attributes[key] = value;
            }
        }

        return attributes;
    }

    [GeneratedRegex(@"(\w+(?:-\w+)?)=(""([^""]*)""|'([^']*)'|([^ ]+))")]
    private static partial Regex KeyValueRegex();
}