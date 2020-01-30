using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Tests
{
    [TestClass]
    public class NotifyPropertyChangedBaseTests
    {
        [TestMethod]
        public void PropertyChangedTest()
        {
            bool changed = false;
            var testclass = new TestClass();
            testclass.PropertyChanged += (s, e) =>
            {
                changed = true;
            };

            testclass.Property = true;
            Assert.IsTrue(changed);
        }
        [TestMethod]
        public void PropertyNotChangedTest()
        {
            bool changed = false;
            var testclass = new TestClass();
            testclass.PropertyChanged += (s, e) =>
            {
                changed = true;
            };

            testclass.Property = false;
            Assert.IsFalse(changed);

            testclass.Property = true;
            Assert.IsTrue(changed);
        }

        public class TestClass : NotifyPropertyChangedBase
        {
            private bool _property;
            public bool Property { get => _property; set => ChangeProperty(value, ref _property); }
        }
    }
}
