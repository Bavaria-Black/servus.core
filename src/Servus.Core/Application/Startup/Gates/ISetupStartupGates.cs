using Servus.Core.Threading.Tasks;

namespace Servus.Core.Application.Startup.Gates;

public interface ISetupStartupGates
{
    void OnRegisterStartupGates(IActionRegistry<IStartupGate> registry);
}