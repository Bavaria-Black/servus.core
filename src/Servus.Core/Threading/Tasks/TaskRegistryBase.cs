using Servus.Core.Collections;

namespace Servus.Core.Threading.Tasks;

public abstract class TaskRegistryBase<T> : ActionRegistry<T> where T : ITaskMarker
{
    
}