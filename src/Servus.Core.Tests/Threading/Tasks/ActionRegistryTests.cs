using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Servus.Core.Collections;
using Servus.Core.Threading.Tasks;
using Xunit;

namespace Servus.Core.Tests.Threading.Tasks;

public class ActionRegistryTests
{
    public class TestInjectable
    {
        public static Task RunAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
        
        public static Task<T> RunAsync<T>(T value, CancellationToken token)
        {
            return Task.FromResult(value);
        }
    }
    
    public class TestTask : IAsyncTask, IAsyncTask<bool>
    {
        private readonly TestInjectable _injectable;

        public TestTask(TestInjectable injectable)
        {
            _injectable = injectable;
        }
        
        public async ValueTask RunAsync(CancellationToken token) => await TestInjectable.RunAsync(token);
        async ValueTask<bool> IAsyncTask<bool>.RunAsync(CancellationToken token) => await TestInjectable.RunAsync(true, token);
    }
    
    [Fact]
    public async Task RegisterTaskTest()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<TestInjectable>();
        var registry = new ActionRegistry<IAsyncTask>();
        registry.Register<TestTask>();

        var app = builder.Build();
        
        var cts = new CancellationTokenSource();
        await registry.RunAsyncParallel(app.Services, (f,c) => f.RunAsync(c), cts.Token);
        await registry.RunAllAsync(app.Services, (f, t) => f.RunAsync(t), cts.Token);
    }
    
    [Fact]
    public async Task RegisterAsyncTaskTest()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<TestInjectable>();
        var registry = new ActionRegistry<IAsyncTask<bool>, bool>();
        registry.Register<TestTask>();

        var app = builder.Build();
        var cts = new CancellationTokenSource();
        
        var any = await registry.RunAllAsync(app.Services, cts.Token).AnyAsync();
        var all = await registry.RunAllAsync(app.Services, cts.Token).AllAsync();
        
        Assert.True(any);
        Assert.True(all);
    }
}