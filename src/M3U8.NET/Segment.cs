namespace M3U8.NET;

public sealed record Segment(
    double Duration,
    string? Title,
    string Uri,
    IReadOnlyDictionary<string, string> Attributes
);