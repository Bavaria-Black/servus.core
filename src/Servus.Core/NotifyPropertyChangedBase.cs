﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Servus.Core;

public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Changes the value of the field and invokes the <see cref="PropertyChanged"/> event,
    /// but only if the values are different.
    /// </summary>
    /// <param name="value">New value</param>
    /// <param name="target">Reference to the target field</param>
    /// <param name="propertyName">Optional property name. Else the name of the calling property is used.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>True if property was changed, otherwise false.</returns>
    /// <example>
    /// This sample shows how to call the <see cref="ChangeProperty{T}" /> method.
    /// <code>
    /// class TestClass 
    /// {
    ///     private bool _myProperty;
    ///     public bool MyProperty 
    ///     { 
    ///         set 
    ///         {
    ///             if (ChangeProperty(value, ref _myProperty)) { DoSomething(); }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    protected bool ChangeProperty<T>(T value, ref T target, [CallerMemberName] string propertyName = "")
    {
        if (value == null || value.Equals(target)) return false;
        
        target = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Changes the value of the field and invokes the <see cref="PropertyChanged"/> event as well as the given callback action,
    /// but only if the values are different.
    /// </summary>
    /// <param name="value">New value</param>
    /// <param name="target">Reference to the target field</param>
    /// <param name="changedCallback">Action to be invoked if value has changed</param>
    /// <param name="propertyName">Optional property name. Else the name of the calling property is used.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>True if property was changed, otherwise false.</returns>
    /// <example>
    /// This sample shows how to call the <see cref="ChangeProperty{T}" /> method.
    /// <code>
    /// class TestClass 
    /// {
    ///     private bool _myProperty;
    ///     public bool MyProperty 
    ///     { 
    ///         set 
    ///         {
    ///             if (ChangeProperty(value, ref _myProperty)) { DoSomething(); }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    protected bool ChangeProperty<T>(T value, ref T target, Action changedCallback, [CallerMemberName] string propertyName = "")
    {
        if (value == null || value.Equals(target)) return false;
        target = value;
        OnPropertyChanged(propertyName);
        changedCallback?.Invoke();
        
        return true;
    }

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Optional property name. Else the name of the calling member is used.</param>
    /// <remarks>In most cases it's advisable to use <see cref="ChangeProperty{T}" /> instead.</remarks>
    /// <example>
    /// This sample shows how to call the <see cref="OnPropertyChanged" /> method.
    /// <code>
    /// class TestClass 
    /// {
    ///     private bool _myProperty;
    ///     public bool MyProperty
    ///     {
    ///         set
    ///         {
    ///            _myProperty = value;
    ///            OnPropertyChanged();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}