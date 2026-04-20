# IWithId

A marker interface for entities that have a `Guid` identifier. It's deliberately minimal — something you can check for, dispatch on, or constrain generics against, without pulling in a heavier domain abstraction.

```csharp
public interface IWithId
{
    Guid Id { get; }
}
```

## Usage

```csharp
using Servus.Core;

public class Order : IWithId
{
    public Guid Id { get; init; } = Guid.NewGuid();
    // …
}

public void Log<T>(T entity) where T : IWithId
    => logger.LogInformation("Processed {Type} {Id}", typeof(T).Name, entity.Id);
```

Combine with [`InvokeIf`](/modules/type-system/conditional-invoke) to check for identifiability at runtime:

```csharp
message.InvokeIf<IWithId>(e => diagnostics.Attach("entity.id", e.Id));
```
