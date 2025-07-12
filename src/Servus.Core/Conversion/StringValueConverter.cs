using System;

namespace Servus.Core.Conversion;

public sealed class StringValueConverter : IStringValueConverter
{
    public Type OutputType  => typeof(string);
    public object? Convert(string value) => value;
}