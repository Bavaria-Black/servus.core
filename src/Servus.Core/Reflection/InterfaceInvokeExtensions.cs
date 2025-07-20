namespace Servus.Core.Reflection;

public static class InterfaceInvokeExtensions
{
    /// <summary>
    /// Conditionally executes an action if the object implements the specified interface or type.
    /// </summary>
    /// <typeparam name="TTarget">The target interface or type to check for.</typeparam>
    /// <param name="entity">The object to check and potentially invoke the action on.</param>
    /// <param name="action">The action to execute if the object is of the target type.</param>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <remarks>
    /// If the entity does not implement TTarget, the method returns without executing the action.
    /// </remarks>
    public static void InvokeIf<TTarget>(this object entity, Action<TTarget> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return;
        action(ntt);
    }
    
    /// <summary>
    /// Conditionally executes an asynchronous action if the object implements the specified interface or type.
    /// </summary>
    /// <typeparam name="TTarget">The target interface or type to check for.</typeparam>
    /// <param name="entity">The object to check and potentially invoke the action on.</param>
    /// <param name="action">The asynchronous action to execute if the object is of the target type.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <remarks>
    /// If the entity does not implement TTarget, the method returns a completed task without executing the action.
    /// </remarks>
    public static async Task InvokeIfAsync<TTarget>(this object entity, Func<TTarget, Task> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return;
        await action(ntt);
    }
    
    /// <summary>
    /// Conditionally executes a function and returns its result if the object implements the specified interface or type.
    /// </summary>
    /// <typeparam name="TTarget">The target interface or type to check for.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="entity">The object to check and potentially invoke the function on.</param>
    /// <param name="action">The function to execute if the object is of the target type.</param>
    /// <returns>The result of the function if the entity is of the target type; otherwise, the default value of TResult.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <remarks>
    /// If the entity does not implement TTarget, the method returns the default value of TResult.
    /// </remarks>
    public static TResult? InvokeIf<TTarget, TResult>(this object entity, Func<TTarget, TResult> action)
        where TTarget : class
    {
        return entity is not TTarget ntt ? default : action(ntt);
    }
    
    /// <summary>
    /// Conditionally executes an asynchronous function and returns its result if the object implements the specified interface or type.
    /// </summary>
    /// <typeparam name="TTarget">The target interface or type to check for.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="entity">The object to check and potentially invoke the function on.</param>
    /// <param name="action">The asynchronous function to execute if the object is of the target type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the function result if the entity is of the target type; otherwise, the default value of TResult.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <remarks>
    /// If the entity does not implement TTarget, the method returns a completed task with the default value of TResult.
    /// </remarks>
    public static async Task<TResult?> InvokeIfAsync<TTarget, TResult>(this object entity, Func<TTarget, Task<TResult>> action)
        where TTarget : class
    {
        if (entity is not TTarget ntt) return default;
        return await action(ntt);
    }
}