using System;

namespace Servus.Core.Conversion;

public interface IValueConverter
{
    Type OutputType { get; }
    
    public object? Convert(string value);
}