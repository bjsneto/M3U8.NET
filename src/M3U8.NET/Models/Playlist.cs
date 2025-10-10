namespace M3U8.NET.Models;

public class Playlist
{
    public string Version { get; set; } 
    public Dictionary<string, string> SessionData { get; set; } = [];
    public List<Segment> Segments { get; set; } = [];
}