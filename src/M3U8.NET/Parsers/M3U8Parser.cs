using M3U8.NET.Constants;
using M3U8.NET.Exceptions;
using M3U8.NET.Parsers.Interfaces;
using System.Text.RegularExpressions;

namespace M3U8.NET.Parsers;

internal partial class M3U8Parser : IPlaylistParser
{
    public Playlist Parse(string content)
    {
        var playlist = new Playlist();

        using var reader = new StringReader(content);

        string? line = reader.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(line) || line != Tag.EXTM3U)
        {
            throw new InvalidM3U8FormatException($"Playlist must start with {Tag.EXTM3U}.");
        }

        Segment? currentSegment = null;

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line) || char.IsControl(line[0])) 
                continue; 

            if (line.StartsWith(Tag.EXTXSESSIONDATA))
            {
                var dataPart = line[Tag.EXTXSESSIONDATA.Length..];
                var attributes = ParseAttributes(dataPart);
                foreach (var attr in attributes)
                {
                    playlist.SessionData[attr.Key] = attr.Value.Trim('"');
                }
            }
            else if (line.StartsWith(Tag.EXTINF))
            {
                currentSegment = new Segment();
                var infPart = line[Tag.EXTINF.Length..];
                var parts = infPart.Split([','], 2);

                if (parts.Length >= 1)
                {
                    var durationAndAttrs = parts[0].Trim();
                    var durationStr = durationAndAttrs.Split(' ')[0];
                    if (!double.TryParse(durationStr, out double duration))
                    {
                        throw new InvalidM3U8FormatException($"Invalid duration format: {durationStr}");
                    }
                    currentSegment.Duration = duration;

                    var attrsStr = string.Join(" ", durationAndAttrs.Split(' ').Skip(1));
                    currentSegment.Attributes = ParseAttributes(attrsStr);
                }

                if (parts.Length == 2)
                {
                    currentSegment.Title = parts[1].Trim();
                }

                var nextLine = reader.ReadLine()?.Trim();
                currentSegment.Uri = !string.IsNullOrWhiteSpace(nextLine) && !nextLine.StartsWith(value: "#") 
                    ? nextLine :
                    throw new InvalidM3U8FormatException("Missing URI after #EXTINF.");

                playlist.Segments.Add(currentSegment);
            }
        }

        return playlist;
    }

    private static Dictionary<string, string> ParseAttributes(string attrString)
    {
        var attributes = new Dictionary<string, string>();
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