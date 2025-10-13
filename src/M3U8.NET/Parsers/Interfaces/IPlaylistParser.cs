namespace M3U8.NET.Parsers.Interfaces;

public interface IPlaylistParser
{
    Playlist Parse(string content);
}
