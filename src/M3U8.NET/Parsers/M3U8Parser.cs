using M3U8.NET.Exceptions;
using M3U8.NET.Models;
using System.Text.RegularExpressions;

namespace M3U8.NET.Parsers;

internal partial class M3U8Parser
{
    private M3U8Parser() { }

    internal static M3U8Parser Create() 
    {
        return new M3U8Parser();
    }
    public Playlist Parse(string content)
    {
        var playlist = new Playlist();

        using var reader = new StringReader(content);

        string? line = reader.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(line) || line != "#EXTM3U")
        {
            throw new InvalidM3U8FormatException("Playlist must start with #EXTM3U.");
        }

        Segment? currentSegment = null;

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line) || char.IsControl(line[0])) 
                continue; 

            if (line.StartsWith("#EXT-X-SESSION-DATA:"))
            {
                var dataPart = line.Substring("#EXT-X-SESSION-DATA:".Length);
                var attributes = ParseAttributes(dataPart);
                foreach (var attr in attributes)
                {
                    playlist.SessionData[attr.Key] = attr.Value.Trim('"');
                }
            }
            else if (line.StartsWith("#EXTINF:"))
            {
                currentSegment = new Segment();
                var infPart = line.Substring("#EXTINF:".Length);
                var parts = infPart.Split(new[] { ',' }, 2);

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
                if (!string.IsNullOrWhiteSpace(nextLine) && !nextLine.StartsWith("#"))
                {
                    currentSegment.Uri = nextLine;
                }
                else
                {
                    throw new InvalidM3U8FormatException("Missing URI after #EXTINF.");
                }

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