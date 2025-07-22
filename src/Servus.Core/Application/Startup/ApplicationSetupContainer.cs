using Microsoft.AspNetCore.Builder;

namespace Servus.Core.Application.Startup;

public abstract class ApplicationSetupContainer<THost> : ApplicationSetupContainer
    where THost : IApplicationBuilder
{
    protected sealed override void SetupApplication(IApplicationBuilder app)
    {
        SetupApplication((THost)app);        
    }
    
    protected abstract void SetupApplication(THost app);
}