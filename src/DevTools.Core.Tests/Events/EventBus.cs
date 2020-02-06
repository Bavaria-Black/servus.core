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
        public void PublishWithSingleSubscriberIsReceived()
        {
            
        }
    }
}