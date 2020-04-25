using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Events;

namespace Servus.Core.Tests.Events
{
    [TestClass]
    public class SynchronousEventBusTests : EventBusTestsBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            EventBus = new SynchronousEventBus();
        }
        
        [TestMethod]
        [Timeout(1000)]
        public void Publish1000EventsWithPredicates()
        {
            const int maxCount = 1000;
            int calledCountA = 0;
            int calledCountB = 0;
            
            EventBus.Subscribe<int>(message =>
            {
                calledCountA++;
            }, i  => i%2 == 0 );
            EventBus.Subscribe<int>(message =>
            {
                calledCountB++;
            }, i  => i%2 != 0 );

            for (int i = 0; i < maxCount; i++)
            {
                EventBus.Publish(i);
            }

            Assert.AreEqual(maxCount/2, calledCountA);
            Assert.AreEqual(maxCount/2, calledCountB);
        }

        [TestMethod]
        [Timeout(20 * 1000)]
        public void Publish1000EventsWithPredicatesMultipleTimes()
        {
            for (int i = 0; i < 100; i++)
            {
                Publish1000EventsWithPredicates();
            }
        }
    }
}