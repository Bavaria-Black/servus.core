using Microsoft.Extensions.DependencyInjection;

namespace Servus.Core.Concurrency.Tasks;

public abstract class TaskRegistryBase<T> : ITaskRegistry<T> where T : ITaskMarker
{
    private readonly List<T> _resolvedTasks = [];
    private readonly List<Type> _taskTypes = [];
    
    public void Register<TTask>() where TTask : T
    {
        _taskTypes.Add(typeof(TTask));
    }

    public void Register(T instance)
    {
        _resolvedTasks.Add(instance);
    }
    
    public IEnumerable<T> GetStartupTasks(IServiceProvider serviceProvider)
    {
        foreach (var taskType in _taskTypes)
        {
            var arguments = taskType
                .GetConstructors()
                .First()
                .GetParameters()
                .Select(p => serviceProvider.GetRequiredService(p.ParameterType))
                .ToArray();
            
            yield return (T)Activator.CreateInstance(taskType, arguments)!;
        }

        foreach (var task in _resolvedTasks) yield return task;
    }
}