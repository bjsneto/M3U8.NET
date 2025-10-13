using Bogus;

namespace M3U8.NET.UnitTests.Builders;

public class SegmentBuilder
{
    private readonly Segment _segment = new();

    public SegmentBuilder()
    {
        var faker = new Faker();
        _segment.Duration = -1;
        _segment.Title = faker.Random.String2(10);
        _segment.Uri = faker.Internet.Url();
        _segment.Attributes = [];
    }

    public SegmentBuilder WithDuration(double duration)
    {
        _segment.Duration = duration;
        return this;
    }

    public SegmentBuilder WithTitle(string title)
    {
        _segment.Title = title;
        return this;
    }

    public SegmentBuilder WithUri(string uri)
    {
        _segment.Uri = uri;
        return this;
    }

    public SegmentBuilder WithAttribute(string key, string value)
    {
        _segment.Attributes[key] = value;
        return this;
    }

    public Segment Build() => _segment;
}