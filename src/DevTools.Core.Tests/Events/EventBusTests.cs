using System;
using System.Threading;
using System.Threading.Tasks;
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
        [Timeout(10000)]
        public void PublishWithoutSubscribersDoesNothing()
        {
            _eventBus.Publish("Payload: Leberkas Semme");
        }

        [TestMethod]
        [Timeout(10000)]
        public void PublishWithSingleSubscriberIsReceived()
        {
            bool called = false;
            var manualResetEvent = new ManualResetEvent(false);
            
            _eventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                {
                    called = true;
                    manualResetEvent.Set();
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            _eventBus.Publish( "test");
            manualResetEvent.WaitOne();
            Assert.IsTrue(called);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void PublishWithMultipleSubscriberAreReceived()
        {
            int calledCount = 0;
            var barrier = new Barrier(3);
            
            _eventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                {
                    Interlocked.Increment(ref calledCount);
                    barrier.RemoveParticipant();
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
                    barrier.RemoveParticipant();
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            _eventBus.Publish("test");

            // Wait until the two subscriptions have been processed
            barrier.SignalAndWait();
            
            Assert.AreEqual(2, calledCount);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void SubscribeUnsubscribeSingleSubscriber()
        {
            int calledCount = 0;
            
            var subscriptionId = _eventBus.Subscribe<string>(message =>
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
            _eventBus.Unsubscribe<string>(subscriptionId);
            
            // Should not trigger the subscriber
            _eventBus.Publish("test"); 
            
            Assert.AreEqual(1, calledCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public void UnsubscribeWhenNotSubscribedDoesNotFail()
        {
            var subscriptionId1 = _eventBus.Subscribe<object>(Console.WriteLine);
            var subscriptionId2 = _eventBus.Subscribe<object>(Console.WriteLine);
            
            _eventBus.Unsubscribe<object>(subscriptionId2);
            _eventBus.Unsubscribe<object>(subscriptionId1);
            _eventBus.Unsubscribe<object>(subscriptionId1);
            _eventBus.Unsubscribe<object>(subscriptionId1);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void SubscribeWithPredicateOnlyReturnsFilteredEvents()
        {
            int calledCount = 0;
            var manualResetEvent = new ManualResetEvent(false);
            
            _eventBus.Subscribe<string>(message =>
            {
                Assert.Fail("Should not have been called.");
            }, s => s.Equals("NotCalled"));
            _eventBus.Subscribe<string>(message =>
            {
                calledCount++;
                manualResetEvent.Set();
            }, s => s.StartsWith("test"));
            
            _eventBus.Publish( "test1");
            _eventBus.Publish( "_test");

            manualResetEvent.WaitOne();
            Assert.AreEqual(1, calledCount);
        }

        
        /// <summary>
        /// Run Publish1000EventsWithPredicates to detect possible threading issues
        /// </summary>
        [TestMethod]
        [Ignore]
        public void ContinuouslyPublish1000EventsWithPredicates()
        {
            // ToDo: Remove test?
            for (int i = 0; i < 10; i++)
            {
                // Console.WriteLine(i);
                Publish1000EventsWithPredicates();
            }
        }
        //ToDo: test blocking subscription action
        
        [TestMethod]
        [Timeout(20*1000)]
        public void Publish1000EventsWithPredicates()
        {
            const int maxCount = 1000;
            var barrier = new Barrier(maxCount + 1); // todo: replace this with an async barrier
            int calledCountA = 0;
            int calledCountB = 0;
            
            _eventBus.Subscribe<int>(message =>
            {
                Interlocked.Increment(ref calledCountA);
                barrier.RemoveParticipant();
            }, i  => i%2 == 0 );
            _eventBus.Subscribe<int>(message =>
            {
                Interlocked.Increment(ref calledCountB);
                barrier.RemoveParticipant();
            }, i  => i%2 != 0 );

            for (int i = 0; i < maxCount; i++)
            {
                _eventBus.Publish(i);
            }

            // Wait until all the 1000 messages have been received
            barrier.SignalAndWait();
            
            Assert.AreEqual(maxCount/2, calledCountA);
            Assert.AreEqual(maxCount/2, calledCountB);
        }
    }
}