using FluentAssertions;
using System.Text;

namespace M3U8.NET.UnitTests.Parsers;

public class PlaylistAsyncTests
{
    [Fact]
    public async Task ParseSegmentsAsync_Should_Parse_All_Segments()
    {
        // Arrange
        string playlistContent = """
        #EXTM3U
        #EXTINF:10,Segment 1
        http://example.com/seg1.ts
        #EXTINF:5,Segment 2
        http://example.com/seg2.ts
        """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(playlistContent));

        // Act
        var segments = new List<Segment>();
        await foreach (var seg in Playlist.ParseSegmentsAsync(stream, cancellationToken: TestContext.Current.CancellationToken))
            segments.Add(seg);

        // Assert
        segments.Should().HaveCount(2);
        segments[0].Uri.Should().Contain("seg1.ts");
        segments[1].Duration.Should().Be(5);
    }

    [Fact]
    public async Task ParseSegmentsAsync_Should_Handle_Invalid_Entries_Gracefully()
    {
        // Arrange
        string playlistContent = """
        #EXTM3U
        #EXTINF:invalid,NoDuration
        http://example.com/broken.ts
        #EXTINF:7,Valid Segment
        http://example.com/ok.ts
        """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(playlistContent));

        // Act
        var segments = new List<Segment>();
        await foreach (var seg in Playlist.ParseSegmentsAsync(stream, cancellationToken: TestContext.Current.CancellationToken))
            segments.Add(seg);

        // Assert
        segments.Should().ContainSingle(s => s.Uri.Contains("ok.ts"));
    }

    [Fact]
    public async Task ParseSegmentsAsync_Should_Support_Cancellation()
    {
        // Arrange
        string playlistContent = """
        #EXTM3U
        #EXTINF:5,Seg 1
        http://example.com/1.ts
        #EXTINF:5,Seg 2
        http://example.com/2.ts
        """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(playlistContent));
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(5);

        // Act
        var act = async () =>
        {
            await foreach (var _ in Playlist.ParseSegmentsAsync(stream, null, cts.Token))
            {
                await Task.Delay(50, cts.Token);
            }
        };

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}