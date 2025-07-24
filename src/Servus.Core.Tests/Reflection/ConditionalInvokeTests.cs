using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Reflection;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Servus.Core.Tests.Reflection;

[TestClass]
public class ConditionalInvokeTests
{
    internal interface ITestInterface
    {
        public bool TestMethod();
        public Task<bool> TestMethodAsync();
    }

    internal class TestImplementation : BasisClass, ITestInterface
    {
        public bool TestMethod() => true;
        public Task<bool> TestMethodAsync()=> Task.FromResult(true);
    }

    internal class BasisClass
    {
        
    }

    private static BasisClass GetBasisClass() => new TestImplementation();
    
    [TestMethod]
    public void ConditionalInvokeTest()
    {
        var service = GetBasisClass();
        
        service.InvokeIf<ITestInterface>(t => IsTrue(t.TestMethod()));
        IsTrue(service.InvokeIf<ITestInterface, bool>(t => t.TestMethod()));
    }
    
    [TestMethod]
    public async Task ConditionalInvokeAsyncTest()
    {
        var service = GetBasisClass();
        
        await service.InvokeIfAsync<ITestInterface>(async t => IsTrue(await t.TestMethodAsync()));
        IsTrue(await service.InvokeIfAsync<ITestInterface, bool>(async t => await t.TestMethodAsync()));
    }
}