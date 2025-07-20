using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Application.Startup;

namespace Servus.Core.Tests.Application;

public class FailureAppConfigurationTestBase : ApplicationSetupContainer
{
    protected override void SetupApplication(IApplicationBuilder app)
    {
        throw new NotImplementedException();
    }
}

public class AppConfigurationTestBase : ApplicationSetupContainer<WebApplication>
{
    protected override void SetupApplication(WebApplication app)
    {
    }
}

[TestClass]
public class AppStartupTests
{
    [TestMethod]
    public async Task SuccessfulStartup()
    {
        var gateIsOpen = false;
        var cts = new CancellationTokenSource();
        var app = AppBuilder.Create()
            .WithSetup<AppConfigurationTestBase>()
            .WithStartupGate(() =>
            {
                gateIsOpen = !gateIsOpen;
                return Task.FromResult(gateIsOpen);
            })
            .Build();

        Assert.IsFalse(gateIsOpen);
        
        await app.StartAsync(cts.Token);
        
        Assert.IsTrue(gateIsOpen);
        await cts.CancelAsync();
    }


    [TestMethod]
    public async Task FailedStartup()
    {
        var cts = new CancellationTokenSource();
        
        var app = AppBuilder.Create()
            .WithSetup<AppConfigurationTestBase>()
            .WithSetup<FailureAppConfigurationTestBase>()
            .WithStartupGate(() => Task.FromResult(true))
            .Build();
        
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await app.StartAsync(cts.Token));
    }
}