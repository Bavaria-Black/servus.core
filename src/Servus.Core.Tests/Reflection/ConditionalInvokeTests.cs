using System.Threading.Tasks;
using Xunit;
using Servus.Core.Reflection;

namespace Servus.Core.Tests.Reflection;

public class ConditionalInvokeTests
{
    internal interface ITestInterface
    {
        public bool Fact();
        public Task<bool> FactAsync();
    }

    private class TestImplementation : ITestInterface
    {
        public bool Fact() => true;
        public Task<bool> FactAsync()=> Task.FromResult(true);
    }

    private static TestImplementation GetBasisClass() => new();
    
    [Fact]
    public void ConditionalInvokeTest()
    {
        var service = GetBasisClass();
        
        service.InvokeIf<ITestInterface>(t =>  Assert.True(t.Fact()));
        Assert.True(service.InvokeIf<ITestInterface, bool>(t => t.Fact()));
    }
    
    [Fact]
    public async Task ConditionalInvokeAsyncTest()
    {
        var service = GetBasisClass();
        
        await service.InvokeIfAsync<ITestInterface>(async t => Assert.True(await t.FactAsync()));
        Assert.True(await service.InvokeIfAsync<ITestInterface, bool>(async t => await t.FactAsync()));
    }
}