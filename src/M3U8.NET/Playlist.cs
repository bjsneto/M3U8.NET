using M3U8.NET.Parsers;
using M3U8.NET.Parsers.Interfaces;

namespace M3U8.NET;

public class Playlist
{
    public int HlsVersion { get; set; }
    public Dictionary<string, string> SessionData { get; set; } = [];
    public List<Segment> Segments { get; set; } = [];

    public static Playlist LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found : {path}");

        string content = File.ReadAllText(path);
        return ParsePlaylistContent(content);
    }

    public static Playlist LoadFromString(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        return ParsePlaylistContent(content);
    }

    private static Playlist ParsePlaylistContent(string content)
    {
        int version = PlaylistParserFactory.DetectVersion(content);
        IPlaylistParser parserStrategy = PlaylistParserFactory.GetParser(version);
        return parserStrategy.Parse(content);
    }
}