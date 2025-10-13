using FluentAssertions;
using M3U8.NET.Exceptions;
using M3U8.NET.UnitTests.Builders;
using M3U8.NET.UnitTests.Fixtures;

namespace M3U8.NET.UnitTests.Parsers;

public class M3U8ParserTests : IClassFixture<M3U8Fixture>
{
    private readonly M3U8Fixture _fixture;

    public M3U8ParserTests(M3U8Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Parse_Should_Parse_Simple_M3U8_With_Metadata()
    {
        var input = _fixture.SimpleExample1;

        var expectedSegment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Channel One [HD]")
            .WithUri("http://cdn.example.com:80/stream/12345/segment1.ts")
            .WithAttribute("tvg-id", "TV-ID-001")
            .WithAttribute("tvg-name", "Channel One [HD]")
            .WithAttribute("tvg-logo", "http://logo.example/logo1.png")
            .WithAttribute("group-title", "Category A | Movies")
            .Build();

        var result = Playlist.LoadFromString(input); 

        result.Should().NotBeNull();
        result.SessionData.Should().ContainKey("DATA-ID").And.ContainValue("session.id.example");
        result.Segments.Should().HaveCount(1);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment);
    }

    [Fact]
    public void Parse_Should_Parse_Simple_M3U8_Without_Extra_Attributes()
    {
        var input = _fixture.SimpleExample2;

        var expectedSegment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Channel Two [SD]")
            .WithUri("http://stream.example.net:80/live/54321/segment2.ts")
            .Build();

        var result = Playlist.LoadFromString(input);  

        result.Should().NotBeNull();
        result.SessionData.Should().ContainKey("DATA-ID").And.ContainValue("session.id.example");
        result.Segments.Should().HaveCount(1);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment);
    }

    [Fact]
    public void Parse_Should_Throw_Exception_If_Not_Start_With_EXTM3U()
    {
        var invalidInput = @"#EXT-X-SESSION-DATA:DATA-ID=""session.id.example""#EXTINF:-1,Channel Two [SD] http://stream.example.net:80/live/54321/segment2.ts";

        Action act = () => Playlist.LoadFromString(invalidInput); 
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("Playlist must start with #EXTM3U.");
    }

    [Fact]
    public void Parse_Should_Parse_Multi_Segment_M3U8()
    {
        var input = _fixture.MultiSegmentExample;

        var expectedSegment1 = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Channel One [HD]")
            .WithUri("http://cdn.example.com:80/stream/12345/segment1.ts")
            .WithAttribute("tvg-id", "TV-ID-001")
            .WithAttribute("tvg-name", "Channel One [HD]")
            .WithAttribute("tvg-logo", "http://logo.example/logo1.png")
            .WithAttribute("group-title", "Category A | Movies")
            .Build();

        var expectedSegment2 = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Channel Three [FHD]")
            .WithUri("http://vod.example.org:80/stream/67890/segment3.ts")
            .WithAttribute("tvg-id", "TV-ID-002")
            .WithAttribute("tvg-name", "Channel Three [FHD]")
            .WithAttribute("tvg-logo", "http://logo.example/logo2.png")
            .WithAttribute("group-title", "Category B | Series")
            .Build();

        var expectedSegment3 = new SegmentBuilder()
            .WithDuration(10)
            .WithTitle("Test Clip")
            .WithUri("http://example.com/testclip.ts")
            .Build();

        var result = Playlist.LoadFromString(input);

        result.Should().NotBeNull();
        result.Segments.Should().HaveCount(3);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment1);
        result.Segments[1].Should().BeEquivalentTo(expectedSegment2);
        result.Segments[2].Should().BeEquivalentTo(expectedSegment3);
    }
}