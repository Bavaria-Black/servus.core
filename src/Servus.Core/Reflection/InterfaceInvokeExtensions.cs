namespace Servus.Core.Reflection;

public static class InterfaceInvokeExtensions
{
    public static void InvokeIf<TTarget>(this object entity, Action<TTarget> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return;
        action(ntt);
    }
    
    public static async Task InvokeIfAsync<TTarget>(this object entity, Func<TTarget, Task> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return;
        await action(ntt);
    }
    
    public static TResult? InvokeIf<TTarget, TResult>(this object entity, Func<TTarget, TResult> action)
        where TTarget : class
    {
        return entity is not TTarget ntt ? default : action(ntt);
    }
    
    public static async Task<TResult?> InvokeIfAsync<TTarget, TResult>(this object entity, Func<TTarget, Task<TResult>> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return default;
        return await action(ntt);
    }
}