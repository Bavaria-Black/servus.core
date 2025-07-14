namespace Servus.Core.Conversion;

public sealed class BoolValueConverter : IStringValueConverter
{
    private readonly HashSet<string> _trueValues = [];
    private readonly HashSet<string> _falseValues = [];
    public BoolValueConverter()
    {
        _trueValues.Add(bool.TrueString.ToLowerInvariant());
        _falseValues.Add(bool.FalseString.ToLowerInvariant());
    }

    public BoolValueConverter(string[] additionalTrueValues, string[] additionalFalseValues)
        : this()
    {
        _trueValues.AddRange(additionalTrueValues.Select(v => v.ToLowerInvariant()));
        _falseValues.AddRange(additionalFalseValues.Select(v => v.ToLowerInvariant()));
    }
    
    public Type OutputType => typeof(bool);
    public object? Convert(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace", nameof(value));

        var trimmedValue = value.Trim().ToLowerInvariant();
        
        if (_trueValues.Contains(trimmedValue))
            return true;
            
        if (_falseValues.Contains(trimmedValue))
            return false;
            
        throw new FormatException(
            $"Unable to convert '{value}' to boolean. Valid true values: [{string.Join(", ", _trueValues)}]. " +
            $"Valid false values: [{string.Join(", ", _falseValues)}]");
    }
}