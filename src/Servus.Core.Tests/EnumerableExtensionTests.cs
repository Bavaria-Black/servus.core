using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests;

[TestClass]
public class EnumerableExtensionTests
{
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
        var distinctListAB = list.DistinctBy(c => new { c.A, c.B }).ToList();

        // Assert
        Assert.AreEqual(2, distinctListA.Count);
        Assert.AreEqual(2, distinctListB.Count);
        Assert.AreEqual(list.Count - 1, distinctListAB.Count);
    }


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
}