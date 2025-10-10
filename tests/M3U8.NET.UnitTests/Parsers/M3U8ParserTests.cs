using FluentAssertions;
using M3U8.NET.Exceptions;
using M3U8.NET.Models;
using M3U8.NET.Parsers;
using M3U8.NET.UnitTests.Builders;
using M3U8.NET.UnitTests.Fixtures;
using M3U8.NET.Validators;

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
        
        var parser = new M3U8Parser();
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

        
        var result = parser.Parse(input);
        M3U8Validator.Validate(result);

        
        result.Should().NotBeNull();
        result.SessionData.Should().ContainKey("DATA-ID").And.ContainValue("session.id.example");
        result.Segments.Should().HaveCount(1);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment);
    }

    [Fact]
    public void Parse_Should_Parse_Simple_M3U8_Without_Extra_Attributes()
    {
        
        var parser = new M3U8Parser();
        var input = _fixture.SimpleExample2;

        var expectedSegment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Channel Two [SD]")
            .WithUri("http://stream.example.net:80/live/54321/segment2.ts")
            .Build();

        
        var result = parser.Parse(input);
        M3U8Validator.Validate(result);

        
        result.Should().NotBeNull();
        result.SessionData.Should().ContainKey("DATA-ID").And.ContainValue("session.id.example");
        result.Segments.Should().HaveCount(1);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment);
    }

    [Fact]
    public void Parse_Should_Throw_Exception_If_Not_Start_With_EXTM3U()
    {
        
        var parser = new M3U8Parser();
        var invalidInput = @"#EXT-X-SESSION-DATA:DATA-ID=""session.id.example""
#EXTINF:-1,Channel Two [SD]
http://stream.example.net:80/live/54321/segment2.ts";

        
        Action act = () => parser.Parse(invalidInput);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("Playlist must start with #EXTM3U.");
    }

    [Fact]
    public void Parse_Should_Parse_Multi_Segment_M3U8()
    {
        
        var parser = new M3U8Parser();
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

        
        var result = parser.Parse(input);
        M3U8Validator.Validate(result);

        
        result.Should().NotBeNull();
        result.Segments.Should().HaveCount(3);
        result.Segments[0].Should().BeEquivalentTo(expectedSegment1);
        result.Segments[1].Should().BeEquivalentTo(expectedSegment2);
        result.Segments[2].Should().BeEquivalentTo(expectedSegment3);
    }

    [Fact]
    public void Validate_Should_Throw_Exception_If_Playlist_Is_Null()
    {
        
        Action act = () => M3U8Validator.Validate(null);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("Playlist cannot be null.");
    }

    [Fact]
    public void Validate_Should_Throw_Exception_If_No_Segments()
    {
        
        var playlist = new Playlist { Segments = new List<Segment>() };

        
        Action act = () => M3U8Validator.Validate(playlist);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("Playlist must contain at least one segment.");
    }

    [Theory]
    [InlineData(-2)]   
    [InlineData(-3)]   
    [InlineData(-10)]
    public void Validate_Should_Throw_Exception_If_Invalid_Duration(double invalidDuration)
    {
        
        var segment = new SegmentBuilder()
            .WithDuration(invalidDuration)
            .WithTitle("Test")
            .WithUri("http://example.com")
            .Build();
        var playlist = new Playlist { Segments = new List<Segment> { segment } };

        
        Action act = () => M3U8Validator.Validate(playlist);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage($"Invalid duration: {invalidDuration}. Must be -1 or positive.");
    }

    [Fact]
    public void Validate_Should_Throw_Exception_If_Empty_Title()
    {
        
        var segment = new SegmentBuilder()
            .WithTitle(string.Empty)
            .WithUri("http://example.com")
            .Build();
        var playlist = new Playlist { Segments = new List<Segment> { segment } };

        
        Action act = () => M3U8Validator.Validate(playlist);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("Segment title cannot be empty.");
    }

    [Theory]
    [InlineData("invalid_uri")]         
    [InlineData("/relative/path")]      
    [InlineData("ftp://example.com")]  
    [InlineData("https:// ")]           
    public void Validate_Should_Throw_Exception_If_Invalid_Uri(string invalidUri)
    {
        
        var segment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Test")
            .WithUri(invalidUri)
            .Build();
        var playlist = new Playlist { Segments = new List<Segment> { segment } };

        
        Action act = () => M3U8Validator.Validate(playlist);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("*Invalid or insecure URI*");
    }

    [Theory]
    [InlineData("invalid_logo")]         
    [InlineData("ftp://logo.com")]     
    public void Validate_Should_Throw_Exception_If_Invalid_TvgLogo(string invalidLogo)
    {
        
        var segment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Test")
            .WithUri("http://example.com")
            .WithAttribute("tvg-logo", invalidLogo)
            .Build();
        var playlist = new Playlist { Segments = new List<Segment> { segment } };

        
        Action act = () => M3U8Validator.Validate(playlist);
        act.Should().Throw<InvalidM3U8FormatException>()
           .WithMessage("*Invalid tvg-logo URI*");
    }

    [Fact]
    public void Validate_Should_Not_Throw_For_Valid_Playlist()
    {
        
        var segment = new SegmentBuilder()
            .WithDuration(-1)
            .WithTitle("Valid Title")
            .WithUri("https://example.com/stream.ts")
            .WithAttribute("tvg-logo", "http://logo.com/image.png")
            .Build();
        var playlist = new Playlist { Segments = new List<Segment> { segment } };

        
        M3U8Validator.Validate(playlist);
    }
}