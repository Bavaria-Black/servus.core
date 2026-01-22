using System.Linq;
using Xunit;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

public class CircularQueueTests
{
    private CircularQueue<string> _queue = null!;

    public CircularQueueTests()
    {
        _queue = new CircularQueue<string>(3);
    }

    [Fact]
    public void Constructor_WithValidCapacity_CreatesEmptyQueue()
    {
        // Arrange & Act
        var queue = new CircularQueue<int>(5);

        // Assert
        Assert.Equal(0, queue.Count);
        Assert.Equal(5, queue.Capacity);
    }

    [Fact]
    public void Enqueue_SingleItem_AddsItemToQueue()
    {
        // Act
        _queue.Enqueue("A");

        // Assert
        Assert.Equal(1, _queue.Count);
        Assert.True(_queue.Items.Contains("A"));
    }

    [Fact]
    public void Enqueue_MultipleItemsWithinCapacity_AddsAllItems()
    {
        var expected = new[] {"A", "B", "C"};
        
        // Act
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        // Assert
        Assert.Equal(3, _queue.Count);
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
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
        Assert.Equal(3, _queue.Count);
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
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
        Assert.Equal(3, _queue.Count);
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
    public void TryDequeue_EmptyQueue_ReturnsFalseAndDefaultValue()
    {
        // Act
        var result = _queue.TryDequeue(out var item);

        // Assert
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryDequeue_QueueWithItems_ReturnsTrueAndFirstItem()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");

        // Act
        var result = _queue.TryDequeue(out var item);

        // Assert
        Assert.True(result);
        Assert.Equal("A", item);
        Assert.Equal(1, _queue.Count);
    }

    [Fact]
    public void TryDequeue_MultipleDequeues_ReturnsItemsInFIFOOrder()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        // Act & Assert
        Assert.True(_queue.TryDequeue(out var item1));
        Assert.Equal("A", item1);

        Assert.True(_queue.TryDequeue(out var item2));
        Assert.Equal("B", item2);

        Assert.True(_queue.TryDequeue(out var item3));
        Assert.Equal("C", item3);

        Assert.False(_queue.TryDequeue(out var item4));
        Assert.Null(item4);
    }

    [Fact]
    public void TryDequeue_AfterCapacityExceeded_ReturnsRemainingItems()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D"); // Should remove "A"

        // Act & Assert
        Assert.True(_queue.TryDequeue(out var item1));
        Assert.Equal("B", item1);

        Assert.True(_queue.TryDequeue(out var item2));
        Assert.Equal("C", item2);

        Assert.True(_queue.TryDequeue(out var item3));
        Assert.Equal("D", item3);
    }

    [Fact]
    public void Count_EmptyQueue_ReturnsZero()
    {
        // Assert
        Assert.Equal(0, _queue.Count);
    }

    [Fact]
    public void Count_AfterEnqueue_ReturnsCorrectCount()
    {
        // Act & Assert
        _queue.Enqueue("A");
        Assert.Equal(1, _queue.Count);

        _queue.Enqueue("B");
        Assert.Equal(2, _queue.Count);

        _queue.Enqueue("C");
        Assert.Equal(3, _queue.Count);
    }

    [Fact]
    public void Count_AfterCapacityExceeded_ReturnsCapacity()
    {
        // Act
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D");
        _queue.Enqueue("E");

        // Assert
        Assert.Equal(3, _queue.Count);
    }

    [Fact]
    public void Count_AfterDequeue_DecreasesCorrectly()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");

        // Act & Assert
        _queue.TryDequeue(out _);
        Assert.Equal(1, _queue.Count);

        _queue.TryDequeue(out _);
        Assert.Equal(0, _queue.Count);
    }

    [Fact]
    public void Items_EmptyQueue_ReturnsEmptyEnumerable()
    {
        // Assert
        Assert.False(_queue.Items.Any());
    }

    [Fact]
    public void Items_WithItems_ReturnsItemsInCorrectOrder()
    {
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");

        var expected = new[] { "A", "B", "C" };

        // Assert
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
    public void Items_AfterCapacityExceeded_ReturnsRemainingItems()
    {
        var expected = new[] {"B", "C", "D"};
        // Arrange
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        _queue.Enqueue("C");
        _queue.Enqueue("D");

        // Assert
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
    public void EnqueueDequeue_MixedOperations_MaintainsCorrectState()
    {
        // Act & Assert
        _queue.Enqueue("A");
        _queue.Enqueue("B");
        Assert.Equal(2, _queue.Count);

        _queue.TryDequeue(out var item1);
        Assert.Equal("A", item1);
        Assert.Equal(1, _queue.Count);

        _queue.Enqueue("C");
        _queue.Enqueue("D");
        Assert.Equal(3, _queue.Count);

        _queue.TryDequeue(out var item2);
        Assert.Equal("B", item2);
        Assert.Equal(2, _queue.Count);

        var expected = new[] {"C", "D"};
        Assert.Equal(expected, _queue.Items.ToArray());
    }

    [Fact]
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
        Assert.Equal(2, intQueue.Count);
        Assert.Equal(expected, intQueue.Items.ToArray());
    }

    [Fact]
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
        Assert.Equal(2, queue.Count);
        var items = queue.Items.ToArray();
        Assert.Equal(2, items[0].Id);
        Assert.Equal(3, items[1].Id);
    }

    [Fact]
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
        Assert.Equal(1000, largeQueue.Count);
        Assert.Equal(500, largeQueue.Items.First()); // Should start from 500
        Assert.Equal(1499, largeQueue.Items.Last());  // Should end at 1499
    }
}

// Test helper class
public class TestObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}