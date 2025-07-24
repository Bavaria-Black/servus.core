namespace Servus.Core.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[TestClass]
public class InsertAtExtensionTests
{
    [TestMethod]
    public void InsertAt_Array_AtBeginning_ShouldInsertCorrectly()
    {
        // Arrange
        var array = new[] {2, 3, 4};

        // Act
        var result = array.InsertAt(0, 1).ToArray();

        // Assert
        CollectionAssert.AreEqual(new[] {1, 2, 3, 4}, result);
    }

    [TestMethod]
    public void InsertAt_Array_AtMiddle_ShouldInsertCorrectly()
    {
        // Arrange
        var array = new[] {1, 2, 4, 5};

        // Act
        var result = array.InsertAt(2, 3).ToArray();

        // Assert
        CollectionAssert.AreEqual(new[] {1, 2, 3, 4, 5}, result);
    }

    [TestMethod]
    public void InsertAt_Array_AtEnd_ShouldInsertCorrectly()
    {
        // Arrange
        var array = new[] {1, 2, 3};

        // Act
        var result = array.InsertAt(3, 4).ToArray();

        // Assert
        CollectionAssert.AreEqual(new[] {1, 2, 3, 4}, result);
    }

    [TestMethod]
    public void InsertAt_Array_BeyondEnd_ShouldThrowException()
    {
        // Arrange
        var array = new[] {1, 2, 3};

        // Assert
        Assert.ThrowsExactly<ArgumentException>(() => array.InsertAt(10, 4));
    }

    [TestMethod]
    public void InsertAt_Array_NegativeIndex_ShouldThrow()
    {
        // Arrange
        var array = new[] {2, 3, 4};

        // Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.InsertAt(-1, 1));
    }

    [TestMethod]
    public void InsertAt_EmptyArray_ShouldCreateSingleElementArray()
    {
        // Arrange
        var array = Array.Empty<int>();
        var expected = new[] {42};

        // Act
        var result = array.InsertAt(0, 42).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_List_AtMiddle_ShouldInsertCorrectly()
    {
        // Arrange
        var list = new List<int> {1, 2, 4, 5};
        var expected = new[] {1, 2, 3, 4, 5};

        // Act
        var result = list.InsertAt(2, 3).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_List_AtBeginning_ShouldInsertCorrectly()
    {
        // Arrange
        var list = new List<string> {"b", "c", "d"};
        var expected = new[] {"a", "b", "c", "d"};
        
        // Act
        var result = list.InsertAt(0, "a").ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_Collection_ShouldInsertCorrectly()
    {
        // Arrange
        var collection = new Collection<int> {1, 3, 4};
        var expected = new[] {1, 2, 3, 4};
        
        // Act
        var result = collection.InsertAt(1, 2).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_GenericEnumerable_ShouldInsertCorrectly()
    {
        // Arrange
        var enumerable = Enumerable.Range(1, 3).Where(x => x != 2); // [1, 3]
        var expected = new[] {1, 2, 3};
        
        // Act
        var result = enumerable.InsertAt(1, 2).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_EmptyEnumerable_ShouldCreateSingleElement()
    {
        // Arrange
        var enumerable = Enumerable.Empty<int>();
        var expected = new[] {42};

        // Act
        var result = enumerable.InsertAt(0, 42).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_WithReferenceTypes_ShouldWorkCorrectly()
    {
        // Arrange
        var strings = new[] {"apple", "cherry", "date"};
        var expected = new[] {"apple", "banana", "cherry", "date"};
        
        // Act
        var result = strings.InsertAt(1, "banana").ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }


    [TestMethod]
    public void InsertAt_OriginalCollectionUnmodified_ShouldNotChangeOriginal()
    {
        // Arrange
        var original = new[] {1, 2, 3};
        var originalCopy = new[] {1, 2, 3};
        var expected = new[] {1, 10, 2, 3};

        // Act
        var result = original.InsertAt(1, 10).ToArray();

        // Assert
        CollectionAssert.AreEqual(originalCopy, original); // Original unchanged
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InsertAt_LargeArray_ShouldPerformCorrectly()
    {
        // Arrange
        var largeArray = Enumerable.Range(0, 1000).ToArray();

        // Act
        var result = largeArray.InsertAt(500, -1).ToArray();

        // Assert
        Assert.AreEqual(1001, result.Length);
        Assert.AreEqual(-1, result[500]);
        Assert.AreEqual(499, result[499]);
        Assert.AreEqual(500, result[501]);
    }

    [TestMethod]
    public void InsertAt_DifferentTypes_ArrayVsList_ShouldProduceSameResult()
    {
        // Arrange
        var array = new[] {1, 2, 4, 5};
        var list = new List<int> {1, 2, 4, 5};

        // Act
        var arrayResult = array.InsertAt(2, 3).ToArray();
        var listResult = list.InsertAt(2, 3).ToArray();

        // Assert
        CollectionAssert.AreEqual(arrayResult, listResult);
    }

    [TestMethod]
    public void InsertAt_CustomCollection_ShouldUseCollectionPath()
    {
        // Arrange
        var customCollection = new CustomCollection<int> {1, 3, 5};
        var expected = new[] {1, 2, 3, 5};

        // Act
        var result = customCollection.InsertAt(1, 2).ToArray();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }

    // Helper class to test ICollection<T> path
    private class CustomCollection<T> : ICollection<T>
    {
        private readonly List<T> _items = [];

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public void Add(T item) => _items.Add(item);
        public void Clear() => _items.Clear();
        public bool Contains(T item) => _items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public bool Remove(T item) => _items.Remove(item);
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}