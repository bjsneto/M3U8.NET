namespace M3U8.NET.Exceptions;

public class M3U8ParseException : Exception
{
    public int LineNumber { get; }
    public string? RawLine { get; }

    public M3U8ParseException(string message, int lineNumber = -1, string? rawLine = null, Exception? inner = null)
        : base(message, inner)
    {
        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    public override string ToString()
        => $"M3U8ParseException (Line {LineNumber}): {Message}\nRaw: {RawLine}";
}
