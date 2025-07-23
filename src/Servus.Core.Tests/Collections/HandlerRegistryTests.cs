using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests.Collections;

[TestClass]
public class HandlerRegistryTests
{
    private HandlerRegistry<string> _registry = new();
    private List<string> _handledItems = [];

    [TestInitialize]
    public void Setup()
    {
        _registry = new HandlerRegistry<string>();
        _handledItems = [];
    }

    [TestMethod]
    public void Register_WithValidParameters_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        Assert.AreEqual(1, _registry.Count);
    }

    [TestMethod]
    public void Register_WithNullCanHandle_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            _registry.Register(null!, s => _handledItems.Add(s)));
    }

    [TestMethod]
    public void Register_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            _registry.Register(s => true, null!));
    }

    [TestMethod]
    public void Handle_WithMatchingHandler_ShouldExecuteHandlerAndReturnTrue()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add($"handled: {s}"));
        
        // Act
        var result = _registry.Handle("test item");
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, _handledItems.Count);
        Assert.AreEqual("handled: test item", _handledItems[0]);
    }

    [TestMethod]
    public void Handle_WithNoMatchingHandler_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.Handle("other item");
        
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, _handledItems.Count);
    }

    [TestMethod]
    public void Handle_WithMultipleMatchingHandlers_ShouldExecuteOnlyFirst()
    {
        // Arrange
        _registry.Register(s => s.Contains("test"), _ => _handledItems.Add("first"));
        _registry.Register(s => s.Contains("test"), _ => _handledItems.Add("second"));
        
        // Act
        var result = _registry.Handle("test item");
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, _handledItems.Count);
        Assert.AreEqual("first", _handledItems[0]);
    }

    [TestMethod]
    public void HandleAll_WithMultipleMatchingHandlers_ShouldExecuteAllAndReturnCount()
    {
        // Arrange
        _registry.Register(s => s.Contains("test"), s => _handledItems.Add("first"));
        _registry.Register(s => s.Contains("test"), s => _handledItems.Add("second"));
        _registry.Register(s => s.StartsWith("other"), s => _handledItems.Add("third"));
        
        // Act
        var result = _registry.HandleAll("test item");
        
        // Assert
        Assert.AreEqual(2, result);
        Assert.AreEqual(2, _handledItems.Count);
        CollectionAssert.AreEqual(new[] { "first", "second" }, _handledItems);
    }

    [TestMethod]
    public void HandleAll_WithNoMatchingHandlers_ShouldReturnZero()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.HandleAll("other item");
        
        // Assert
        Assert.AreEqual(0, result);
        Assert.AreEqual(0, _handledItems.Count);
    }

    [TestMethod]
    public void CanHandle_WithMatchingHandler_ShouldReturnTrue()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.CanHandle("test item");
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, _handledItems.Count);
    }

    [TestMethod]
    public void CanHandle_WithNoMatchingHandler_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.CanHandle("other item");
        
        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Count_ShouldReturnNumberOfRegisteredHandlers()
    {
        // Arrange & Act
        Assert.AreEqual(0, _registry.Count);
        
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        Assert.AreEqual(1, _registry.Count);
        
        _registry.Register(s => s.StartsWith("other"), s => _handledItems.Add(s));
        Assert.AreEqual(2, _registry.Count);
    }

    [TestMethod]
    public void Clear_ShouldRemoveAllHandlersAndClearStash()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        _registry.Stash();
        _registry.Register(s => s.StartsWith("other"), s => _handledItems.Add(s));
        
        // Act
        _registry.Clear();
        
        // Assert
        Assert.AreEqual(0, _registry.Count);
        Assert.IsFalse(_registry.Pop()); // Stash should be empty
    }

    [TestMethod]
    public void GetMatchingHandlers_ShouldReturnAllMatchingHandlers()
    {
        // Arrange
        _registry.Register(s => s.Contains("test"), s => _handledItems.Add("first"));
        _registry.Register(s => s.Contains("test"), s => _handledItems.Add("second"));
        _registry.Register(s => s.StartsWith("other"), s => _handledItems.Add("third"));
        
        // Act
        var handlers = _registry.GetMatchingHandlers("test item").ToList();
        
        // Assert
        Assert.AreEqual(2, handlers.Count);
        
        // Execute handlers to verify they're correct
        foreach (var handler in handlers)
        {
            handler("test item");
        }
        CollectionAssert.AreEqual(new[] { "first", "second" }, _handledItems);
    }

    [TestMethod]
    public void Stash_ShouldSaveCurrentHandlersAndClearRegistry()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add("original"));
        
        // Act
        _registry.Stash();
        
        // Assert
        Assert.AreEqual(0, _registry.Count);
        Assert.IsFalse(_registry.CanHandle("test item"));
    }

    [TestMethod]
    public void Pop_WithStashedHandlers_ShouldRestoreHandlersAndReturnTrue()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add("original"));
        _registry.Stash();
        _registry.Register(s => s.StartsWith("new"), s => _handledItems.Add("new"));
        
        // Act
        var result = _registry.Pop();
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, _registry.Count);
        Assert.IsTrue(_registry.CanHandle("test item"));
        Assert.IsFalse(_registry.CanHandle("new item"));
    }

    [TestMethod]
    public void Pop_WithEmptyStash_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.Pop();
        
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(1, _registry.Count); // Original handlers should remain
    }

    [TestMethod]
    public void StashAndPop_ShouldWorkWithMultipleLevels()
    {
        // Arrange
        _registry.Register(s => s == "level1", s => _handledItems.Add("level1"));
        _registry.Stash();
        
        _registry.Register(s => s == "level2", s => _handledItems.Add("level2"));
        _registry.Stash();
        
        _registry.Register(s => s == "level3", s => _handledItems.Add("level3"));
        
        // Act & Assert
        Assert.IsTrue(_registry.CanHandle("level3"));
        Assert.IsFalse(_registry.CanHandle("level2"));
        Assert.IsFalse(_registry.CanHandle("level1"));
        
        _registry.Pop();
        Assert.IsTrue(_registry.CanHandle("level2"));
        Assert.IsFalse(_registry.CanHandle("level1"));
        Assert.IsFalse(_registry.CanHandle("level3"));
        
        _registry.Pop();
        Assert.IsTrue(_registry.CanHandle("level1"));
        Assert.IsFalse(_registry.CanHandle("level2"));
        Assert.IsFalse(_registry.CanHandle("level3"));
        
        Assert.IsFalse(_registry.Pop()); // No more stashed handlers
    }

    [TestMethod]
    public void StashAndPop_ShouldReplaceCurrentHandlers()
    {
        // Arrange
        _registry.Register(s => s == "original", s => _handledItems.Add("original"));
        _registry.Stash();
        _registry.Register(s => s == "temp1", s => _handledItems.Add("temp1"));
        _registry.Register(s => s == "temp2", s => _handledItems.Add("temp2"));
        
        // Act
        _registry.Pop();
        
        // Assert
        Assert.AreEqual(1, _registry.Count);
        Assert.IsTrue(_registry.CanHandle("original"));
        Assert.IsFalse(_registry.CanHandle("temp1"));
        Assert.IsFalse(_registry.CanHandle("temp2"));
    }
}