# NotifyPropertyChangedBase

An abstract base class that gives you the standard `INotifyPropertyChanged` implementation, plus a `ChangeProperty` helper that does equality-check + set + raise-event in one call. Useful for view-models and settings objects.

## Usage

```csharp
using Servus.Core;

public class UserViewModel : NotifyPropertyChangedBase
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => ChangeProperty(value, ref _name);
    }

    private int _age;
    public int Age
    {
        get => _age;
        set => ChangeProperty(value, ref _age);
    }
}
```

`ChangeProperty` uses `EqualityComparer<T>.Default` — so `PropertyChanged` only fires when the value actually changed.

## With a callback

If you also need to run custom logic when the value changes:

```csharp
public string Theme
{
    get => _theme;
    set => ChangeProperty(value, ref _theme, () => ApplyTheme(value));
}
```

## Protected surface

```csharp
public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool ChangeProperty<T>(T value, ref T target,
        [CallerMemberName] string propertyName = "");

    protected bool ChangeProperty<T>(T value, ref T target, Action changedCallback,
        [CallerMemberName] string propertyName = "");

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "");
}
```

The `[CallerMemberName]` attribute means you don't have to pass the property name explicitly — the compiler fills it in with the name of the caller (your property setter).
