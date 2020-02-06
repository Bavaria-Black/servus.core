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
        public void ChangePropertyTriggersPropertyChangedEvent()
        {
            bool changed = false;
            var testClass = new TestClass();
            testClass.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TestClass.Property))
                {
                    changed = true;
                }
            };

            testClass.Property = true;
            Assert.IsTrue(changed);
        }
        
        [TestMethod]
        public void ChangePropertyDoesNotTriggerPropertyChangedEventForSameValue()
        {
            bool changed = false;
            var testClass = new TestClass();
            testClass.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TestClass.Property))
                {
                    changed = true;
                }
            };

            testClass.Property = false;
            Assert.IsFalse(changed);

            testClass.Property = true;
            Assert.IsTrue(changed);
        }
      
        [TestMethod]
        public void OnPropertyChangedUtilizesCallerMemberName()
        {
            bool changed = false;
            var testClass = new TestClass();
            testClass.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TestClass.OtherProperty))
                {
                    changed = true;
                }
            };

            testClass.OtherProperty = true;
            Assert.IsTrue(changed);
        }

        private class TestClass : NotifyPropertyChangedBase
        {
            private bool _property;
            public bool Property { set => ChangeProperty(value, ref _property); }
            public bool OtherProperty
            {
                set
                {
                    _property = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
