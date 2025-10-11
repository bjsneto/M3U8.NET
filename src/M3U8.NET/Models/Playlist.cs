using M3U8.NET.Parsers;
using M3U8.NET.Validators;

namespace M3U8.NET.Models;

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
        var parser = M3U8Parser.Create();
        var playlist = parser.Parse(content);
        M3U8Validator.Validate(playlist);
        return playlist;
    }
}