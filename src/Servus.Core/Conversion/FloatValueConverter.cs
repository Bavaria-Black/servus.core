using System;
using System.Globalization;

namespace Servus.Core.Conversion;

public sealed class FloatValueConverter : IValueConverter
{
    public Type OutputType => typeof(float);
    public object? Convert(string value) => float.Parse(value, CultureInfo.InvariantCulture);
}