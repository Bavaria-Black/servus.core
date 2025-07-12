using System;

namespace Servus.Core.Conversion;

public sealed class StringValueConverter : IValueConverter
{
    public Type OutputType  => typeof(string);
    public object? Convert(string value) => value;
}