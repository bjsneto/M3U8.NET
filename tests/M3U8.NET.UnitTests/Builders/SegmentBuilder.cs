using Bogus;
using Bogus.DataSets;
using System.Collections.ObjectModel;

namespace M3U8.NET.UnitTests.Builders;

public class SegmentBuilder
{
    private readonly Faker faker = new Faker();
    private double _duration;
    private string? _title;
    private string _uri;
    private readonly Dictionary<string, string> _attributes;

    public SegmentBuilder()
    {
        _duration = -1;
        _title = faker.Random.String2(10);
        _uri = faker.Internet.Url();
        _attributes = new Dictionary<string, string>();
    }

    public SegmentBuilder WithDuration(double duration)
    {
        _duration = duration;
        return this;
    }

    public SegmentBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public SegmentBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public SegmentBuilder WithAttribute(string key, string value)
    {
        _attributes[key] = value;
        return this;
    }

    public Segment Build()
    {
        return new Segment(
            _duration,
            _title,
            _uri,
            new ReadOnlyDictionary<string, string>(_attributes)
        );
    }
}