using System.Globalization;

namespace Servus.Core.Conversion;

public sealed class DoubleValueConverter : IStringValueConverter
{
    public Type OutputType => typeof(double);
    public object? Convert(string value) => double.Parse(value, CultureInfo.InvariantCulture);
}