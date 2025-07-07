using Servus.Core.Threading.Tasks;

namespace Servus.Core.Application.Startup.Tasks;

public interface ISetupStartupGates
{
    void OnRegisterStartupGates(IActionRegistry<IStartupGate> registry);
}