using Servus.Core.Collections;
using Servus.Core.Threading.Tasks;

namespace Servus.Core.Application.Startup.Tasks;

public interface ISetupPreStartupTasks
{
    void OnRegisterPreStartupTasks(IActionRegistry<IAsyncTask> registry);
}