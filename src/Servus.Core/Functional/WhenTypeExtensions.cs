namespace Servus.Core.Functional;

public static class TypeExtensions
{
    public static void WhenType<T>(this object target, Action<T> handler)
    {
        if(target is T t) handler(t);
    }
}