namespace Servus.Core.Threading.Tasks;

public interface ITaskRegistry<in T> where T : ITaskMarker
{
    void Register<TTask>() where TTask : T;
    void Register(T instance);
}