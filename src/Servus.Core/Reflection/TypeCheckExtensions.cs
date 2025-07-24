namespace Servus.Core.Reflection;

public static class TypeCheckExtensions
{
    public static bool TryConvert<TTarget>(this object? item, out TTarget? value)
    {
        value = default;
        if (item is not TTarget ntt) return false;

        value = ntt;
        return true;
    }

    public static TTarget Convert<TTarget>(this object? item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return (TTarget) item;
    }

    public static TTarget? Convert<TTarget>(this object? item, Func<object, TTarget> converter)
        => Convert<object, TTarget>(item, converter);

    public static TTarget? Convert<TInput, TTarget>(this object? item, Func<TInput, TTarget> converter)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(converter);

        return item.InvokeIf(converter);
    }
}