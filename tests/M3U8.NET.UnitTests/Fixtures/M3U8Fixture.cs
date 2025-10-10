namespace M3U8.NET.UnitTests.Fixtures;

public class M3U8Fixture : IClassFixture<M3U8Fixture>
{
    public string SimpleExample1 { get; } = @"#EXTM3U
#EXT-X-SESSION-DATA:DATA-ID=""session.id.example""
#EXTINF:-1 tvg-id=""TV-ID-001"" tvg-name=""Channel One [HD]"" tvg-logo=""http://logo.example/logo1.png"" group-title=""Category A | Movies"",Channel One [HD]
http://cdn.example.com:80/stream/12345/segment1.ts";

    public string SimpleExample2 { get; } = @"#EXTM3U
#EXT-X-SESSION-DATA:DATA-ID=""session.id.example""
#EXTINF:-1,Channel Two [SD]
http://stream.example.net:80/live/54321/segment2.ts";

    public string MultiSegmentExample { get; } = @"#EXTM3U
#EXT-X-SESSION-DATA:DATA-ID=""session.id.example""
#EXTINF:-1 tvg-id=""TV-ID-001"" tvg-name=""Channel One [HD]"" tvg-logo=""http://logo.example/logo1.png"" group-title=""Category A | Movies"",Channel One [HD]
http://cdn.example.com:80/stream/12345/segment1.ts
#EXTINF:-1 tvg-id=""TV-ID-002"" tvg-name=""Channel Three [FHD]"" tvg-logo=""http://logo.example/logo2.png"" group-title=""Category B | Series"",Channel Three [FHD]
http://vod.example.org:80/stream/67890/segment3.ts
#EXTINF:10,Test Clip
http://example.com/testclip.ts";
}