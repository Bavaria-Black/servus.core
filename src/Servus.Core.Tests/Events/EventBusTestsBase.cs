using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Events;

namespace Servus.Core.Tests.Events
{
    public abstract class EventBusTestsBase
    {
        protected EventBus EventBus;
        
        public abstract void Initialize();

        [TestMethod]
        [Timeout(10000)]
        public void PublishWithoutSubscribersDoesNothing()
        {
            EventBus.Publish("Payload: Leberkas Semme");
        }

        [TestMethod]
        [Timeout(10000)]
        public void PublishWithSingleSubscriberIsReceived()
        {
            bool called = false;
            var manualResetEvent = new ManualResetEvent(false);
            
            EventBus.Subscribe<string>(message =>
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
            
            EventBus.Publish( "test");
            manualResetEvent.WaitOne();
            Assert.IsTrue(called);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void PublishWithMultipleSubscriberAreReceived()
        {
            int calledCount = 0;
            var barrier = new Barrier(3);
            
            EventBus.Subscribe<string>(message =>
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
            
            EventBus.Subscribe<string>(message =>
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
            
            EventBus.Publish("test");

            // Wait until the two subscriptions have been processed
            barrier.SignalAndWait();
            
            Assert.AreEqual(2, calledCount);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void SubscribeUnsubscribeSingleSubscriber()
        {
            int calledCount = 0;
            var manualResetEvent = new ManualResetEvent(false);
            var barrier = new Barrier(2);
            
            var subscriptionId = EventBus.Subscribe<string>(message =>
            {
                if (message == "test")
                {
                    calledCount++;
                    barrier.RemoveParticipant();
                }
                else
                {
                    Assert.Fail("Should not have been called.");
                }
            });
            
            EventBus.Publish("test");
            barrier.SignalAndWait();
            
            EventBus.Unsubscribe<string>(subscriptionId);
            
            // Should not trigger the subscriber
            EventBus.Publish("test");
            

            
            Assert.AreEqual(1, calledCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public void UnsubscribeWhenNotSubscribedDoesNotFail()
        {
            var subscriptionId1 = EventBus.Subscribe<object>(Console.WriteLine);
            var subscriptionId2 = EventBus.Subscribe<object>(Console.WriteLine);
            
            EventBus.Unsubscribe<object>(subscriptionId2);
            EventBus.Unsubscribe<object>(subscriptionId1);
            EventBus.Unsubscribe<object>(subscriptionId1);
            EventBus.Unsubscribe<object>(subscriptionId1);
        }
        
        [TestMethod]
        [Timeout(10000)]
        public void SubscribeWithPredicateOnlyReturnsFilteredEvents()
        {
            int calledCount = 0;
            var manualResetEvent = new ManualResetEvent(false);
            
            EventBus.Subscribe<string>(message =>
            {
                Assert.Fail("Should not have been called.");
            }, s => s.Equals("NotCalled"));
            EventBus.Subscribe<string>(message =>
            {
                calledCount++;
                manualResetEvent.Set();
            }, s => s.StartsWith("test"));
            
            EventBus.Publish( "test1");
            EventBus.Publish( "_test");

            manualResetEvent.WaitOne();
            Assert.AreEqual(1, calledCount);
        }
    }
}