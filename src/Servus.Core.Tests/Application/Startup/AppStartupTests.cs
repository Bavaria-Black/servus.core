using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Servus.Core.Application.Startup;
using Xunit;

namespace Servus.Core.Tests.Application.Startup;

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

public class HostBuilderSetupContainer : IHostBuilderSetupContainer
{
    public bool WasCalled { get; set; }

    public void ConfigureHostBuilder(IHostBuilder builder)
    {
        WasCalled = true;
    }
}

public class AppStartupTests
{
    private static AppBuilder CreateTestAppBuilder()
    {
        var builder = WebApplication.CreateBuilder(["--urls", "http://127.0.0.1:0"]);
        return AppBuilder.Create(builder, b => b.Build());
    }

    [Fact]
    public async Task SuccessfulStartup()
    {
        var gateIsOpen = false;
        var cts = new CancellationTokenSource();
        var app = CreateTestAppBuilder()
            .WithSetup<AppConfigurationTestBase>()
            .WithStartupGate(() =>
            {
                gateIsOpen = !gateIsOpen;
                return Task.FromResult(gateIsOpen);
            })
            .Build();

        Assert.False(gateIsOpen);

        await app.StartAsync(cts.Token);

        Assert.True(gateIsOpen);
        await cts.CancelAsync();
    }

    [Fact]
    public async Task FailedStartup()
    {
        var cts = new CancellationTokenSource();

        var app = CreateTestAppBuilder()
            .WithSetup<AppConfigurationTestBase>()
            .WithSetup<FailureAppConfigurationTestBase>()
            .WithStartupGate(() => Task.FromResult(true))
            .Build();

        await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await app.StartAsync(cts.Token));
    }

    [Fact]
    public void HostBuilderSetup()
    {
        var container = new HostBuilderSetupContainer();
        _ = AppBuilder.Create()
            .WithSetup(container)
            .Build();

        Assert.True(container.WasCalled);
    }
}