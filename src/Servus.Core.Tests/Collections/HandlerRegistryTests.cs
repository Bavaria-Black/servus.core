using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

public class HandlerRegistryTests
{
    private HandlerRegistry _registry = new();
    private List<string> _handledItems = [];

    public HandlerRegistryTests()
    {
        _registry = new HandlerRegistry();
        _handledItems = [];
    }

    [Fact]
    public void Register_WithoutCondition_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        _registry.Register<string>(s => _handledItems.Add(s));
        
        Assert.Equal(1, _registry.Count);
        Assert.True(_registry.Handle("test"));
        Assert.Single(_handledItems);
    }

    [Fact]
    public void Register_WithValidParameters_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        Assert.Equal(1, _registry.Count);
    }

    [Fact]
    public void Register_WithNullCanHandle_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _registry.Register<string>(null!, s => _handledItems.Add(s)));
    }

    [Fact]
    public void Register_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _registry.Register<string>(_ => true, null!));
    }

    [Fact]
    public void Handle_WithMatchingHandler_ShouldExecuteHandlerAndReturnTrue()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add($"handled: {s}"));
        
        // Act
        var result = _registry.Handle("test item");
        
        // Assert
        Assert.True(result);
        Assert.Equal(1, _handledItems.Count);
        Assert.Equal("handled: test item", _handledItems[0]);
    }

    [Fact]
    public void Handle_WithNoMatchingHandler_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.Handle("other item");
        
        // Assert
        Assert.False(result);
        Assert.Equal(0, _handledItems.Count);
    }

    [Fact]
    public void Handle_WithMultipleMatchingHandlers_ShouldExecuteOnlyFirst()
    {
        // Arrange
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("first"));
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("second"));
        
        // Act
        var result = _registry.Handle("test item");
        
        // Assert
        Assert.True(result);
        Assert.Equal(1, _handledItems.Count);
        Assert.Equal("first", _handledItems[0]);
    }

    [Fact]
    public void Handle_AnyHandler_WithTypeObject()
    {
        // Arrange
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("first"));
        _registry.Register<object>(_ => true, o =>_handledItems.Add("any"));
        _registry.Register<string>(s => s.Contains("leberkas"), _ => _handledItems.Add("second"));
        
        // Act
        var result = _registry.Handle("test item");
        var resultAny = _registry.Handle("leberkas");
        
        // Assert
        Assert.True(result);
        Assert.Equal(2, _handledItems.Count);
        Assert.Equal("first", _handledItems[0]);
        
        Assert.True(resultAny);
        Assert.Equal("any", _handledItems[1]);
    }

    [Fact]
    public void HandleAll_WithMultipleMatchingHandlers_ShouldExecuteAllAndReturnCount()
    {
        // Arrange
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("first"));
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("second"));
        _registry.Register<string>(s => s.StartsWith("other"), _ => _handledItems.Add("third"));
        var expected = new[] {"first", "second"};
        
        // Act
        var result = _registry.HandleAll("test item");
        
        // Assert
        Assert.Equal(2, result);
        Assert.Equal(2, _handledItems.Count);
        Assert.Equal(expected, _handledItems);
    }

    [Fact]
    public void HandleAll_WithNoMatchingHandlers_ShouldReturnZero()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.HandleAll("other item");
        
        // Assert
        Assert.Equal(0, result);
        Assert.Equal(0, _handledItems.Count);
    }

    [Fact]
    public void CanHandle_WithMatchingHandler_ShouldReturnTrue()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.CanHandle("test item");
        
        // Assert
        Assert.True(result);
        Assert.Equal(0, _handledItems.Count);
    }

    [Fact]
    public void CanHandle_WithNoMatchingHandler_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.CanHandle("other item");
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Count_ShouldReturnNumberOfRegisteredHandlers()
    {
        // Arrange & Act
        Assert.Equal(0, _registry.Count);
        
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        Assert.Equal(1, _registry.Count);
        
        _registry.Register<string>(s => s.StartsWith("other"), s => _handledItems.Add(s));
        Assert.Equal(2, _registry.Count);
    }

    [Fact]
    public void Clear_ShouldRemoveAllHandlersAndClearStash()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        _registry.Stash();
        _registry.Register<string>(s => s.StartsWith("other"), s => _handledItems.Add(s));
        
        // Act
        _registry.Clear();
        
        // Assert
        Assert.Equal(0, _registry.Count);
        Assert.False(_registry.Pop()); // Stash should be empty
    }

    [Fact]
    public void GetMatchingHandlers_ShouldReturnAllMatchingHandlers()
    {
        // Arrange
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("first"));
        _registry.Register<string>(s => s.Contains("test"), _ => _handledItems.Add("second"));
        _registry.Register<string>(s => s.StartsWith("other"), _ => _handledItems.Add("third"));
        var expected = new[] {"first", "second"};
        
        // Act
        var handlers = _registry.GetMatchingHandlers("test item").ToList();
        
        // Assert
        Assert.Equal(2, handlers.Count);
        
        // Execute handlers to verify they're correct
        foreach (var handler in handlers)
        {
            handler("test item");
        }
        Assert.Equal(expected, _handledItems);
    }

    [Fact]
    public void Stash_ShouldSaveCurrentHandlersAndClearRegistry()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), _ => _handledItems.Add("original"));
        
        // Act
        _registry.Stash();
        
        // Assert
        Assert.Equal(0, _registry.Count);
        Assert.False(_registry.CanHandle("test item"));
    }

    [Fact]
    public void Pop_WithStashedHandlers_ShouldRestoreHandlersAndReturnTrue()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), _ => _handledItems.Add("original"));
        _registry.Stash();
        _registry.Register<string>(s => s.StartsWith("new"), _ => _handledItems.Add("new"));
        
        // Act
        var result = _registry.Pop();
        
        // Assert
        Assert.True(result);
        Assert.Equal(1, _registry.Count);
        Assert.True(_registry.CanHandle("test item"));
        Assert.False(_registry.CanHandle("new item"));
    }

    [Fact]
    public void Pop_WithEmptyStash_ShouldReturnFalse()
    {
        // Arrange
        _registry.Register<string>(s => s.StartsWith("test"), s => _handledItems.Add(s));
        
        // Act
        var result = _registry.Pop();
        
        // Assert
        Assert.False(result);
        Assert.Equal(1, _registry.Count); // Original handlers should remain
    }

    [Fact]
    public void StashAndPop_ShouldWorkWithMultipleLevels()
    {
        // Arrange
        _registry.Register<string>(s => s == "level1", _ => _handledItems.Add("level1"));
        _registry.Stash();
        
        _registry.Register<string>(s => s == "level2", _ => _handledItems.Add("level2"));
        _registry.Stash();
        
        _registry.Register<string>(s => s == "level3", _ => _handledItems.Add("level3"));
        
        // Act & Assert
        Assert.True(_registry.CanHandle("level3"));
        Assert.False(_registry.CanHandle("level2"));
        Assert.False(_registry.CanHandle("level1"));
        
        _registry.Pop();
        Assert.True(_registry.CanHandle("level2"));
        Assert.False(_registry.CanHandle("level1"));
        Assert.False(_registry.CanHandle("level3"));
        
        _registry.Pop();
        Assert.True(_registry.CanHandle("level1"));
        Assert.False(_registry.CanHandle("level2"));
        Assert.False(_registry.CanHandle("level3"));
        
        Assert.False(_registry.Pop()); // No more stashed handlers
    }

    [Fact]
    public void StashAndPop_ShouldReplaceCurrentHandlers()
    {
        // Arrange
        _registry.Register<string>(s => s == "original", _ => _handledItems.Add("original"));
        _registry.Register<int>(s => true, s => Assert.Equal(555, s));
        _registry.Stash();
        _registry.Register<string>(s => s == "temp1", _ => _handledItems.Add("temp1"));
        _registry.Register<string>(s => s == "temp2", _ => _handledItems.Add("temp2"));
        
        // Act
        _registry.Pop();
        
        // Assert
        Assert.Equal(2, _registry.Count);
        Assert.True(_registry.CanHandle("original"));
        Assert.False(_registry.CanHandle("temp1"));
        Assert.False(_registry.CanHandle("temp2"));
        Assert.True(_registry.CanHandle(555));
    }
}