using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

[TestClass]
public class CircularQueueTests
{
    private CircularQueue<string> _queue = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _queue = new CircularQueue<string>(3);
    }

    [TestMethod]
    public void Constructor_WithValidCapacity_CreatesEmptyQueue()
    {
        // Arrange & Act
        var queue = new CircularQueue<int>(5);

        // Assert
        Assert.AreEqual(0, queue.Count);
        Assert.AreEqual(5, queue.Capacity);
    }

    [TestMethod]
    public void Enqueue_SingleItem_AddsItemToQueue()
    {
        // Act
        _queue.Enqueue("A");

        // Assert
        Assert.AreEqual(1, _queue.Count);
        Assert.IsTrue(_queue.Items.Contains("A"));
    }

    [TestMethod]
    public void Enqueue_MultipleItemsWithinCapacity_AddsAllItems()
    {
        var expected = new[] {"A", "B", "C"};
        
        // Act
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        // Assert
        Assert.AreEqual(3, _queue.Count);
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void Enqueue_ItemsExceedingCapacity_RemovesOldestItems()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        var expected = new[] {"B", "C", "D"};

        // Act
        _queue.Enqueue("D");

        // Assert
        Assert.AreEqual(3, _queue.Count);
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void Enqueue_MultipleItemsExceedingCapacity_MaintainsCapacityAndOrder()
    {
        var expected = new[] {"C", "D", "E"};
        // Act
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D");
        _queue.Enqueue("E");

        // Assert
        Assert.AreEqual(3, _queue.Count);
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void TryDequeue_EmptyQueue_ReturnsFalseAndDefaultValue()
    {
        // Act
        var result = _queue.TryDequeue(out var item);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(item);
    }

    [TestMethod]
    public void TryDequeue_QueueWithItems_ReturnsTrueAndFirstItem()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");

        // Act
        var result = _queue.TryDequeue(out var item);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("A", item);
        Assert.AreEqual(1, _queue.Count);
    }

    [TestMethod]
    public void TryDequeue_MultipleDequeues_ReturnsItemsInFIFOOrder()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        // Act & Assert
        Assert.IsTrue(_queue.TryDequeue(out var item1));
        Assert.AreEqual("A", item1);

        Assert.IsTrue(_queue.TryDequeue(out var item2));
        Assert.AreEqual("B", item2);

        Assert.IsTrue(_queue.TryDequeue(out var item3));
        Assert.AreEqual("C", item3);

        Assert.IsFalse(_queue.TryDequeue(out var item4));
        Assert.IsNull(item4);
    }

    [TestMethod]
    public void TryDequeue_AfterCapacityExceeded_ReturnsRemainingItems()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D"); // Should remove "A"

        // Act & Assert
        Assert.IsTrue(_queue.TryDequeue(out var item1));
        Assert.AreEqual("B", item1);

        Assert.IsTrue(_queue.TryDequeue(out var item2));
        Assert.AreEqual("C", item2);

        Assert.IsTrue(_queue.TryDequeue(out var item3));
        Assert.AreEqual("D", item3);
    }

    [TestMethod]
    public void Count_EmptyQueue_ReturnsZero()
    {
        // Assert
        Assert.AreEqual(0, _queue.Count);
    }

    [TestMethod]
    public void Count_AfterEnqueue_ReturnsCorrectCount()
    {
        // Act & Assert
        _queue.Enqueue("A");
        Assert.AreEqual(1, _queue.Count);

        _queue.Enqueue("B");
        Assert.AreEqual(2, _queue.Count);

        _queue.Enqueue("C");
        Assert.AreEqual(3, _queue.Count);
    }

    [TestMethod]
    public void Count_AfterCapacityExceeded_ReturnsCapacity()
    {
        // Act
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D");
        _queue.Enqueue("E");

        // Assert
        Assert.AreEqual(3, _queue.Count);
    }

    [TestMethod]
    public void Count_AfterDequeue_DecreasesCorrectly()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");

        // Act & Assert
        _queue.TryDequeue(out _);
        Assert.AreEqual(1, _queue.Count);

        _queue.TryDequeue(out _);
        Assert.AreEqual(0, _queue.Count);
    }

    [TestMethod]
    public void Items_EmptyQueue_ReturnsEmptyEnumerable()
    {
        // Assert
        Assert.IsFalse(_queue.Items.Any());
    }

    [TestMethod]
    public void Items_WithItems_ReturnsItemsInCorrectOrder()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        var expected = new[] { "A", "B", "C" };

        // Assert
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void Items_AfterCapacityExceeded_ReturnsRemainingItems()
    {
        var expected = new[] {"B", "C", "D"};
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D");

        // Assert
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void EnqueueDequeue_MixedOperations_MaintainsCorrectState()
    {
        // Act & Assert
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        Assert.AreEqual(2, _queue.Count);

        _queue.TryDequeue(out var item1);
        Assert.AreEqual("A", item1);
        Assert.AreEqual(1, _queue.Count);

        _queue.Enqueue("C");
        _queue.Enqueue("D");
        Assert.AreEqual(3, _queue.Count);

        _queue.TryDequeue(out var item2);
        Assert.AreEqual("B", item2);
        Assert.AreEqual(2, _queue.Count);

        var expected = new[] {"C", "D"};
        CollectionAssert.AreEqual(expected, _queue.Items.ToArray());
    }

    [TestMethod]
    public void CircularQueue_WithDifferentTypes_WorksCorrectly()
    {
        // Arrange
        var intQueue = new CircularQueue<int>(2);
        var expected = new[] {2, 3};

        // Act
        intQueue.Enqueue(1);
        intQueue.Enqueue(2);
        intQueue.Enqueue(3); // Should remove 1

        // Assert
        Assert.AreEqual(2, intQueue.Count);
        CollectionAssert.AreEqual(expected, intQueue.Items.ToArray());
    }

    [TestMethod]
    public void CircularQueue_WithCustomObjects_WorksCorrectly()
    {
        // Arrange
        var queue = new CircularQueue<TestObject>(2);
        var obj1 = new TestObject { Id = 1, Name = "First" };
        var obj2 = new TestObject { Id = 2, Name = "Second" };
        var obj3 = new TestObject { Id = 3, Name = "Third" };

        // Act
        queue.Enqueue(obj1);
        queue.Enqueue(obj2);
        queue.Enqueue(obj3); // Should remove obj1

        // Assert
        Assert.AreEqual(2, queue.Count);
        var items = queue.Items.ToArray();
        Assert.AreEqual(2, items[0].Id);
        Assert.AreEqual(3, items[1].Id);
    }

    [TestMethod]
    public void CircularQueue_LargeCapacity_WorksCorrectly()
    {
        // Arrange
        var largeQueue = new CircularQueue<int>(1000);

        // Act
        for (int i = 0; i < 1500; i++)
        {
            largeQueue.Enqueue(i);
        }

        // Assert
        Assert.AreEqual(1000, largeQueue.Count);
        Assert.AreEqual(500, largeQueue.Items.First()); // Should start from 500
        Assert.AreEqual(1499, largeQueue.Items.Last());  // Should end at 1499
    }
}

// Test helper class
public class TestObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}