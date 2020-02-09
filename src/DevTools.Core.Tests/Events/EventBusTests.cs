using System.Threading;
using DevTools.Core.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevTools.Core.Tests.Events
{
    [TestClass]
    public class EventBusTests
    {
        private EventBus _eventBus;

        [TestInitialize]
        public void Initialize()
        {
            _eventBus = new EventBus();
        }

        [TestMethod]
        [Timeout(100000)]
        public void PublishWithoutSubscribersDoesNothing()
        {
            _eventBus.Publish("Payload: Leberkas Semme");
        }

        [TestMethod]
        [Timeout(100000)]
        public void PublishWithSingleSubscriberIsReceived()
        {
            bool called = false;
            
            _eventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                {
                    called = true;
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            _eventBus.Publish( "test");
            Assert.IsTrue(called);
        }
        
        [TestMethod]
        [Timeout(100000)]
        public void PublishWithMultipleSubscriberAreReceived()
        {
            int calledCount = 0;
            
            _eventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                {
                    Interlocked.Increment(ref calledCount);
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            _eventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                { 
                    Interlocked.Increment(ref calledCount);
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            _eventBus.Publish("test");
            Assert.AreEqual(2, calledCount);
        }
    }
}