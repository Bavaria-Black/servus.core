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
        public void PublishWithoutSubscribersDoesNothing()
        {
            _eventBus.Publish("/food", "Payload: Leberkas Semme");
        }

        [TestMethod]
        [Timeout(100000)]
        public void PublishWithSingleSubscriberIsReceived()
        {
            bool called = false;
            
            _eventBus.Subscribe<string>("/test", message =>
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
            
            _eventBus.Publish("/test", "test");
            Assert.IsTrue(called);
        }
    }
}