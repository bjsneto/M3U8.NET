namespace M3U8.NET.Parsing;

public sealed record ParseOptions
{
    public bool AllowNonStandardTags { get; init; } = true;
    public bool ResolveRelativeUris { get; init; } = true;
    public Uri? BaseUri { get; init; } = null;
    public int? MaxSegmentsToParse { get; init; } = null;
    public bool TrimLines { get; init; } = true;
    public bool RemoveBOM { get; init; } = true;
    public Action<ParseWarning>? OnWarning { get; init; }
}