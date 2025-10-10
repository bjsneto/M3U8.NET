namespace M3U8.NET.Models;

public class Segment
{
    public double Duration { get; set; }
    public string Title { get; set; } 
    public string Uri { get; set; } 
    public Dictionary<string, string> Attributes { get; set; } = [];
}
