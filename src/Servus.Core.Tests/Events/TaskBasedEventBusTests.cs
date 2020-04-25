using System.Threading;
using Servus.Core.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests.Events
{
    [TestClass]
    public class TaskBasedEventBusTests : EventBusTestsBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            EventBus = new TaskBasedEventBus();
        }
        
        [TestMethod]
        [Timeout(10*1000)]
        public void Publish1000EventsWithPredicates()
        {
            const int maxCount = 1000;
            var barrier = new Barrier(maxCount + 1); // todo: replace this with an async barrier
            int calledCountA = 0;
            int calledCountB = 0;
            
            EventBus.Subscribe<int>(message =>
            {
                Interlocked.Increment(ref calledCountA);
                barrier.RemoveParticipant();
            }, i  => i%2 == 0 );
            EventBus.Subscribe<int>(message =>
            {
                Interlocked.Increment(ref calledCountB);
                barrier.RemoveParticipant();
            }, i  => i%2 != 0 );

            for (int i = 0; i < maxCount; i++)
            {
                EventBus.Publish(i);
            }

            // Wait until all the 1000 messages have been received
            barrier.SignalAndWait();
            
            Assert.AreEqual(maxCount/2, calledCountA);
            Assert.AreEqual(maxCount/2, calledCountB);
        }
        

        
        [TestMethod]
        [Timeout(20 * 1000)]
        public void Publish1000EventsWithPredicatesMultipleTimes()
        {
            for (int i = 0; i <1000; i++)
            {
                Initialize();
                Publish1000EventsWithPredicates();
            }
        }
    }
}