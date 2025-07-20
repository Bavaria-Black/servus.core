namespace Servus.Core.Conversion;

public interface IStringValueConverter
{
    Type OutputType { get; }
    
    public object? Convert(string value);
}