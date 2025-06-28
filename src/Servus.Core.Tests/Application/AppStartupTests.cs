using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Application;

namespace Servus.Core.Tests.Application;

public class AppConfigurationTestBase : AppConfigurationBase
{
    public override void SetupServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {

    }
}

public class FailureAppConfigurationTestBase : AppConfigurationBase
{
    public override void SetupServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        throw new NotImplementedException();
    }
}

[TestClass]
public class AppStartupTests
{
    [TestMethod]
    public async Task SuccessfulStartup()
    {
        var cts = new CancellationTokenSource();
        await AppStarter.StartAsync<AppConfigurationTestBase>(cts.Token);
        Assert.IsTrue(true);
        await cts.CancelAsync();
    }
    
    [TestMethod]
    public async Task FailedStartup()
    {
        var cts = new CancellationTokenSource();
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await AppStarter.StartAsync<FailureAppConfigurationTestBase>(cts.Token));
    }
}