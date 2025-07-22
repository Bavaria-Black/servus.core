namespace Servus.Core.Threading.Tasks;

public interface IActionRegistry<T>
{
    /// <summary>
    /// Registers an action type to be resolved through dependency injection when executed.
    /// </summary>
    /// <typeparam name="TImplementation">The concrete implementation type that implements T.</typeparam>
    void Register<TImplementation>() where TImplementation : T;

    /// <summary>
    /// Registers a pre-resolved action instance.
    /// </summary>
    /// <param name="instance">The action instance to register.</param>
    void Register(T instance);
}