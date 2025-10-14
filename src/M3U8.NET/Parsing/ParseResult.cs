namespace M3U8.NET.Parsing;

public sealed record ParseResult<TModel>
{
    public TModel Model { get; init; }
    public IReadOnlyList<ParseWarning> Warnings { get; init; }
    public bool IsSuccess => Warnings.All(w => w.Level != WarningLevel.Error);
    public ParseResult(TModel model, IEnumerable<ParseWarning>? warnings = null)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Warnings = warnings is not null 
            ? warnings.ToList().AsReadOnly() :
            Array.Empty<ParseWarning>();
    }
}