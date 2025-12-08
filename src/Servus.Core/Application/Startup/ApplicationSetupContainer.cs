using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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