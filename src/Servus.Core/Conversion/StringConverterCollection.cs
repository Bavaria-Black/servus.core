using System;
using System.Collections.Generic;
using Servus.Core.Collections;

namespace Servus.Core.Conversion;

public class StringConverterCollection
{
    private readonly Dictionary<Type, IStringValueConverter> _converters = new ();
    private Func<Exception, object?> _exceptionHandler = (e) => null;
    
    public void RegisterExceptionHandler(Func<Exception, object?> handler) => _exceptionHandler = handler;

    public void Register(IStringValueConverter converter, InsertionBehavior behavior = InsertionBehavior.None)
    {
        switch (behavior)
        {
            case InsertionBehavior.OverwriteExisting:
                _converters[converter.OutputType] = converter;
                break;
            case InsertionBehavior.ThrowOnExisting when !_converters.TryAdd(converter.OutputType, converter):
                throw new ArgumentException("Converter already exists", nameof(converter));
            default:
                _converters.TryAdd(converter.OutputType, converter);
                break;
        }
    }

    public object? Convert<T>(string value) => Convert(typeof(T), value);

    public object? Convert(Type targetType, string value)
    {
        if (!_converters.TryGetValue(targetType, out var converter)) return null;

        try
        {
            return converter.Convert(value);
        }
        catch (Exception ex)
        {
            return _exceptionHandler(ex);
        }
    }
}