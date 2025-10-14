namespace M3U8.NET.Parsing;

public sealed record ParseWarning(
    int LineNumber,
    string RawLine,
    string Message,
    WarningLevel Level = WarningLevel.Warning
);
