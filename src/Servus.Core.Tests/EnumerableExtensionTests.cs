using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests;

#nullable disable

[TestClass]
public partial class EnumerableExtensionTests
{
    class DummyClass
    {
        public int A { get; set; }
        public bool B { get; set; }

        public DummyClass(int a, bool b)
        {
            A = a;
            B = b;
        }
    }
    
    [TestMethod]
    public void DistinctByIsWorking()
    {
        // Arrange
        var list = new List<DummyClass>
        {
            new DummyClass(1, true),
            new DummyClass(1, false),
            new DummyClass(2, true),
            new DummyClass(2, false),
            new DummyClass(1, false)    // doubled entry like [1]
        };

        // Act
        var distinctListA = list.DistinctBy(c => c.A).ToList();
        var distinctListB = list.DistinctBy(c => c.B).ToList();
        var distinctListAb = list.DistinctBy(c => new { c.A, c.B }).ToList();

        // Assert
        Assert.AreEqual(2, distinctListA.Count);
        Assert.AreEqual(2, distinctListB.Count);
        Assert.AreEqual(list.Count - 1, distinctListAb.Count);
    }
    
    
        #region GetIndex Tests

        [TestMethod]
        [DataRow(new[] { 1, 2, 3, 4, 5 }, 3, 2)]
        [DataRow(new[] { 1, 2, 3, 4, 5 }, 1, 0)]
        [DataRow(new[] { 1, 2, 3, 4, 5 }, 5, 4)]
        [DataRow(new[] { 42 }, 42, 0)]
        [DataRow(new[] { 1, 2, 2, 3 }, 2, 1)] // First match
        public void GetIndex_ItemExists_ReturnsCorrectIndex(int[] source, int target, int expectedIndex)
        {
            // Act
            var result = source.GetIndex(x => x == target);

            // Assert
            Assert.AreEqual(expectedIndex, result);
        }

        [TestMethod]
        [DataRow(new[] { 1, 2, 3 }, 99)]
        [DataRow(new[] { 1, 2, 3 }, 0)]
        [DataRow(new int[0], 1)]
        public void GetIndex_ItemNotFound_ReturnsMinusOne(int[] source, int target)
        {
            // Act
            var result = source.GetIndex(x => x == target);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIndex_NullSource_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<int> source = null;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.GetIndex(x => x == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIndex_NullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var source = new[] { 1, 2, 3 };

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.GetIndex(null);
        }

        #endregion

        #region ForEach Tests

        [TestMethod]
        [DataRow(new[] { 1, 2, 3 }, new[] { 2, 4, 6 })]
        [DataRow(new[] { 5 }, new[] { 10 })]
        [DataRow(new[] { 0, -1, 2 }, new[] { 0, -2, 4 })]
        public void ForEach_ExecutesActionForEachItem(int[] source, int[] expected)
        {
            // Arrange
            var results = new List<int>();

            // Act
            source.ForEach(x => results.Add(x * 2));

            // Assert
            CollectionAssert.AreEqual(expected, results);
        }

        [TestMethod]
        [DataRow(new int[0])]
        public void ForEach_EmptyCollection_DoesNotExecuteAction(int[] source)
        {
            // Arrange
            var actionExecuted = false;

            // Act
            source.ForEach(_ => actionExecuted = true);

            // Assert
            Assert.IsFalse(actionExecuted);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEach_NullSource_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<int> source = null;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.ForEach(_ => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEach_NullAction_ThrowsArgumentNullException()
        {
            // Arrange
            var source = new[] { 1, 2, 3 };

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.ForEach(null);
        }

        #endregion

        #region ForEachIndexed Tests

        [TestMethod]
        [DataRow(new[] { "a", "b", "c" }, new[] { "(a,0)", "(b,1)", "(c,2)" })]
        [DataRow(new[] { "single" }, new[] { "(single,0)" })]
        [DataRow(new[] { "x", "y" }, new[] { "(x,0)", "(y,1)" })]
        public void ForEachIndexed_ExecutesActionWithCorrectIndexes(string[] source, string[] expected)
        {
            // Arrange
            var results = new List<string>();

            // Act
            source.ForEachIndexed((item, index) => results.Add($"({item},{index})"));

            // Assert
            Assert.IsFalse(source.IsEmpty());
            CollectionAssert.AreEqual(expected, results);
        }

        [TestMethod]
        [DataRow([])]
        public void ForEachIndexed_EmptyCollection_DoesNotExecuteAction(string[] source)
        {
            // Arrange
            var actionExecuted = false;

            // Act
            source.ForEachIndexed((_, _) => actionExecuted = true);

            // Assert
            Assert.IsFalse(actionExecuted);
        }

        [TestMethod]
        public void ForEachIndexed_LargeCollection_IndexesAreSequential()
        {
            // Arrange
            var source = Enumerable.Range(1, 1000);
            var indexes = new List<int>();

            // Act
            source.ForEachIndexed((_, index) => indexes.Add(index));

            // Assert
            var expectedIndexes = Enumerable.Range(0, 1000).ToArray();
            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEachIndexed_NullSource_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<int> source = null;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.ForEachIndexed((_, _) => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEachIndexed_NullAction_ThrowsArgumentNullException()
        {
            // Arrange
            var source = new[] { 1, 2, 3 };

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            source.ForEachIndexed(null);
        }

        #endregion
}