using M3U8.NET.Constants;
using M3U8.NET.Parsers.Interfaces;

namespace M3U8.NET.Parsers;

public static class PlaylistParserFactory
{
    private static readonly Dictionary<int, IPlaylistParser> parsers = new()
    {
        { -1, new M3U8Parser() }
    };

    public static IPlaylistParser GetParser(int version)
    {
        if (parsers.TryGetValue(version, out var parser))
        {
            return parser;
        }
        throw new NotSupportedException($"No parser available for version {version}.");
    }

    public static int DetectVersion(string content)
    {
        ReadOnlySpan<char> versionPrefix = Tag.EXTXVERSION.AsSpan();
        ReadOnlySpan<char> contentSpan = content.AsSpan();   
        int start = 0;

        while (start < contentSpan.Length)
        {
            int end = contentSpan.Slice(start).IndexOfAny('\r', '\n');
            if (end == -1)
            {
                end = contentSpan.Length - start; 
            }

            ReadOnlySpan<char> currentLine = contentSpan.Slice(start, end);

            start += end + 1;
            while (start < contentSpan.Length && (contentSpan[start] == '\r' || contentSpan[start] == '\n'))
            {
                start++;
            }

            if (currentLine.StartsWith(versionPrefix))
            {
                ReadOnlySpan<char> versionValueSpan = currentLine.Slice(versionPrefix.Length).Trim();
                if (int.TryParse(versionValueSpan, out int version))
                {
                    return version;
                }
                throw new NotSupportedException($"No parser available for version {version}.");
            }
        }
        return -1;
    }
}
