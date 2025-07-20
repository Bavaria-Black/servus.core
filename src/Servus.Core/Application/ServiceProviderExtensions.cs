using Microsoft.Extensions.DependencyInjection;

namespace Servus.Core.Application;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Resolves a service of the specified type from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <param name="serviceProvider">The service provider used to resolve the service.</param>
    /// <returns>An instance of type T resolved from the service provider.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service of type T is not registered in the service provider.</exception>
    /// <exception cref="InvalidCastException">Thrown when the resolved service cannot be cast to type T.</exception>
    public static T ResolveExternal<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetRequiredService(typeof(T));
    
    /// <summary>
    /// Resolves and creates an instance of the specified type by automatically injecting its constructor dependencies from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve constructor dependencies.</param>
    /// <param name="type">The type to instantiate.</param>
    /// <returns>An instance of the specified type with all constructor dependencies resolved.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required services cannot be resolved from the service provider or when the type has no constructors.</exception>
    /// <remarks>
    /// This extension method uses the first available constructor of the specified type and resolves all its parameters 
    /// through the service provider's GetRequiredService method before creating the instance using Activator.CreateInstance.
    /// </remarks>
    public static object ResolveExternal(this IServiceProvider serviceProvider, Type type)
    {
        var arguments = type
            .GetConstructors()
            .First()
            .GetParameters()
            .Select(p => serviceProvider.GetRequiredService(p.ParameterType))
            .ToArray();
            
        return Activator.CreateInstance(type, arguments)!;
    }
}