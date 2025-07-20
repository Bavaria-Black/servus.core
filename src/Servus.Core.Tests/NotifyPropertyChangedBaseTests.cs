using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests;

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

    [TestMethod]
    public void ChangePropertyReturnsFalseIfValueNotChanged()
    {
        // Arrange
        var testClass = new TestClass();

        // Act
        testClass.PropertyWithAction = false;

        // Assert
        Assert.IsFalse(testClass.PropertyWithActionCalled);
    }

    [TestMethod]
    public void ChangePropertyReturnsTrueIfValueChanged()
    {
        // Arrange
        var testClass = new TestClass();

        // Act
        testClass.PropertyWithAction = true;

        // Assert
        Assert.IsTrue(testClass.PropertyWithActionCalled);
    }

    [TestMethod]
    public void ChangedCallbackIsNotCalledIfValueHasntChanged()
    {
        // Arrange
        var testClass = new TestClass();

        // Act
        testClass.PropertyWithCallback = false;

        // Assert
        Assert.IsFalse(testClass.PropertyWithCallbackCalled);
    }

    [TestMethod]
    public void ChangedCallbackIsCalledIfValueChanged()
    {
        // Arrange
        var testClass = new TestClass();

        // Act
        testClass.PropertyWithCallback = true;

        // Assert
        Assert.IsTrue(testClass.PropertyWithCallbackCalled);
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

        private bool _propertyWithAction;
        public bool PropertyWithAction
        {
            set
            {
                if (ChangeProperty(value, ref _propertyWithAction))
                {
                    PropertyWithActionCalled = true;
                }
            }
        }

        public bool PropertyWithActionCalled { get; private set; }



        private bool _propertyWithCallback;
        public bool PropertyWithCallback
        {
            set
            {
                ChangeProperty(value, ref _propertyWithCallback, () => PropertyWithCallbackCalled = true);
            }
        }

        public bool PropertyWithCallbackCalled { get; private set; }
    }
}