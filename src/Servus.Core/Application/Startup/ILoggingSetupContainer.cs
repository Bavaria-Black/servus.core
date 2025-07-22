using Microsoft.Extensions.Logging;

namespace Servus.Core.Application.Startup;

public interface ILoggingSetupContainer : ISetupContainer
{
    void SetupLogging(ILoggingBuilder builder);
}