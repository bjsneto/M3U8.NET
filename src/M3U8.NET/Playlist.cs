using M3U8.NET.Parsers;
using M3U8.NET.Parsers.Interfaces;
using M3U8.NET.Parsing;
using System.Runtime.CompilerServices;

namespace M3U8.NET;

public sealed record Playlist(
    int HlsVersion,
    IReadOnlyDictionary<string, string> SessionData,
    IReadOnlyList<Segment> Segments
)
{
    public static Playlist LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        string content = File.ReadAllText(path);
        return ParsePlaylistContent(content);
    }

    public static Playlist LoadFromString(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        return ParsePlaylistContent(content);
    }

    public static async IAsyncEnumerable<Segment> ParseSegmentsAsync(
        Stream stream,
        ParseOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parser = new M3U8Parser();

        await foreach (var seg in parser.ParseSegmentsAsync(stream, options, cancellationToken)
                           .WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
        {
            yield return seg;
        }
    }

    private static Playlist ParsePlaylistContent(string content)
    {
        int version = PlaylistParserFactory.DetectVersion(content);
        IPlaylistParser parserStrategy = PlaylistParserFactory.GetParser(version);
        return parserStrategy.Parse(content);
    }
}