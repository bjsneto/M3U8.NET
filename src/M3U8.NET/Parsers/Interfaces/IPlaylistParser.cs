using M3U8.NET.Parsing;
using System.Runtime.CompilerServices;

namespace M3U8.NET.Parsers.Interfaces;

public interface IPlaylistParser
{
    Playlist Parse(string content);
    Playlist Parse(string content, ParseOptions? options = null);
    bool TryParse(string content, out Playlist? playlist, out IReadOnlyList<ParseWarning> warnings, ParseOptions? options = null);
    IAsyncEnumerable<Segment> ParseSegmentsAsync(Stream input, ParseOptions? options = null, [EnumeratorCancellation] CancellationToken ct = default);
}
