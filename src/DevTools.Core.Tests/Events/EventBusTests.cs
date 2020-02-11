using System;
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
        [Timeout(10000)]
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

            _eventBus.Subscribe<string>(message =>
            {
                Assert.Fail("Should not have been called.");
            }, s => s.Equals("NotCalled"));
            _eventBus.Subscribe<string>(message =>
            {
                calledCount++;
            }, s => s.StartsWith("test"));
            
            _eventBus.Publish( "test1");
            _eventBus.Publish( "_test");

            Assert.AreEqual(1, calledCount);
        }
        
        [TestMethod]
        [Timeout(20000)]
        public void Publish1000EventsWithPredicates()
        {
            int calledCountA = 0;
            int calledCountB = 0;
            
            _eventBus.Subscribe<int>(message =>
            {
                calledCountA++;
            }, i  => i%2 == 0 );
            _eventBus.Subscribe<int>(message =>
            {
                calledCountB++;
            }, i  => i%2 != 0 );

            for (int i = 0; i < 1000; i++)
            {
                _eventBus.Publish(i);
            }

            Assert.AreEqual(500, calledCountA);
            Assert.AreEqual(500, calledCountB);
        }
    }
}