namespace M3U8.NET.Exceptions;

public class InvalidM3U8FormatException : Exception
{
    public InvalidM3U8FormatException(string message) : base(message) { }
}