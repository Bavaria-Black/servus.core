# HandlerRegistry

A flexible collection of conditional handlers — pairs of predicates and actions that run based on the item you pass in. It's the guts of chain-of-responsibility routing, message dispatch, and conditional processing pipelines, without writing the plumbing every time.

## What's in it

Each registered handler is a pair:

- **`CanHandle` predicate** — `Predicate<T>`; returns `true` if this handler should run for the item
- **Handler action** — `Action<T>`; executed when the predicate matches

The registry supports **first-match** dispatch (`Handle`) and **all-matches** dispatch (`HandleAll`), plus a stash/pop mechanism for temporarily swapping handler sets.

## Basic usage

```csharp
using Servus.Core.Collections;

var registry = new HandlerRegistry();

registry.Register<string>(
    canHandle: s => s.StartsWith("ERROR"),
    handler:   s => Console.WriteLine($"Error logged: {s}"));

registry.Register<string>(
    canHandle: s => s.StartsWith("WARN"),
    handler:   s => Console.WriteLine($"Warning logged: {s}"));

registry.Register<string>(
    canHandle: s => s.Contains("URGENT"),
    handler:   s => SendAlert(s));

// First match only — stops after WARN handler
registry.Handle("URGENT WARN: System overload");

// All matches — logs warning AND sends alert
registry.HandleAll("URGENT WARN: System overload");
```

## Stashing handlers (testing shim)

`Stash` saves the current handler set and clears the registry; `Pop` restores the most recently stashed set. Stash/Pop is LIFO, so you can nest them.

```csharp
// Production handlers
registry.Register<string>(s => s.StartsWith("INFO"),  LogToFile);
registry.Register<string>(s => s.StartsWith("ERROR"), LogToDatabase);

registry.Stash();                       // save + clear

// Test handlers
registry.Register<string>(s => s.StartsWith("INFO"),  LogToConsole);
registry.Register<string>(s => s.StartsWith("ERROR"), LogToConsole);

registry.Handle("INFO: Test message");  // goes to console

registry.Pop();                         // restore production handlers

registry.Handle("INFO: Production message"); // back to file
```

## Use cases

### Message routing

```csharp
messageRegistry.Register<Message>(
    msg => msg.Type == MessageType.Command,
    msg => commandProcessor.Process(msg));

messageRegistry.Register<Message>(
    msg => msg.Type == MessageType.Event,
    msg => eventStore.Save(msg));
```

### HTTP request routing

```csharp
routeRegistry.Register<HttpRequest>(
    req => req.Path.StartsWith("/api/users"),
    req => userController.Handle(req));

routeRegistry.Register<HttpRequest>(
    req => req.Path.StartsWith("/api/orders"),
    req => orderController.Handle(req));
```

### Validation pipeline (all-matches)

```csharp
validators.Register<Order>(o => o.Amount > 1000,      o => o.RequiresApproval = true);
validators.Register<Order>(o => o.Customer.IsVip,     o => o.Priority = Priority.High);
validators.Register<Order>(o => o.Items.Count > 50,   o => o.PreferBulkShipping = true);

validators.HandleAll(order); // every matching rule applied
```

## API

```csharp
public class HandlerRegistry
{
    public int Count { get; }

    public void Register<T>(Action<T> handler);
    public void Register<T>(Predicate<T> canHandle, Action<T> handler);
    public void Register(Type type, Predicate<object> canHandle, Action<object> handler);

    public bool Handle(object item);
    public int HandleAll(object item);

    public bool CanHandle(object item);
    public IEnumerable<Action<object>> GetMatchingHandlers(object item);

    public void Clear();
    public void Stash();
    public bool Pop();
}
```
