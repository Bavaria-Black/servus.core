using Servus.Core.Application;


namespace Servus.Core.Collections;

/// <summary>
/// A registry that manages and executes a collection of actions of type T,
/// supporting both type-based registration with dependency injection and instance-based registration.
/// </summary>
/// <typeparam name="T">The base type or interface that all registered actions must implement.</typeparam>
public class ActionRegistry<T>
{
    private readonly List<T> _resolvedTasks = [];
    private readonly List<Type> _taskTypes = [];
        
    /// <summary>
    /// Registers an action type to be resolved through dependency injection when executed.
    /// </summary>
    /// <typeparam name="TImplementation">The concrete implementation type that implements T.</typeparam>
    public void Register<TImplementation>() where TImplementation : T
    {
        _taskTypes.Add(typeof(TImplementation));
    }
    
    /// <summary>
    /// Registers a pre-resolved action instance.
    /// </summary>
    /// <param name="instance">The action instance to register.</param>
    public void Register(T instance)
    {
        _resolvedTasks.Add(instance);
    }
    
    /// <summary>
    /// Gets all registered actions by resolving types through the service provider and combining with pre-resolved instances.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve registered types.</param>
    /// <returns>Enumerable of all registered actions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    protected IEnumerable<T> GetActions(IServiceProvider serviceProvider)
    {
        foreach (var taskType in _taskTypes)
        {
            yield return (T)serviceProvider.ResolveExternal(taskType);
        }

        foreach (var task in _resolvedTasks) yield return task;
    }

    /// <summary>
    /// Executes all registered actions synchronously using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The function that defines how each action should be executed.</param>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    public void RunAll(IServiceProvider sp, Action<T> executor)
    {
        foreach (var action in GetActions(sp))
        {
            executor(action);
        }
    }
    
    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Executes all registered actions asynchronously in parallel using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The async function that defines how each action should be executed.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the parallel execution.</param>
    /// <returns>A ValueTask representing the asynchronous parallel execution of all actions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public async ValueTask RunAsyncParallel(IServiceProvider sp, Func<T, CancellationToken, ValueTask> executor, CancellationToken cancellationToken)
    {
        var actions = GetActions(sp);
        await Parallel.ForEachAsync(actions, cancellationToken, 
            async (action, token) => await executor(action, token));
    }
    
    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Executes all registered actions asynchronously in sequence using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The async function that defines how each action should be executed.</param>
    /// <returns>A ValueTask representing the asynchronous sequential execution of all actions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    public async ValueTask RunAllAsync(IServiceProvider sp, Func<T, ValueTask> executor)
    {
        foreach (var action in GetActions(sp))
        {
            await executor(action).AsTask();
        }
    }
}