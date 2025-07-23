HandlerRegistry
===============

The ``HandlerRegistry<T>`` is a flexible collection that stores conditional
handlers - pairs of predicates and actions that can be executed based on
runtime conditions. It's particularly useful for implementing
chain-of-responsibility patterns, message routing, or conditional processing
workflows.

Overview
--------

Each handler in the registry consists of:

- **CanHandle predicate**: A ``Predicate<T>`` that determines if the handler
  should process an item
- **Handler action**: An ``Action<T>`` that processes the item when the
  predicate matches

The registry supports both "first match" and "all matches" execution patterns,
plus a stashing mechanism for temporarily switching handler sets.

Basic Usage
-----------

.. code-block:: csharp

   var registry = new HandlerRegistry<string>();

   // Register handlers with conditions
   registry.Register(
       canHandle: s => s.StartsWith("ERROR"),
       handler: s => Console.WriteLine($"Error logged: {s}")
   );

   registry.Register(
       canHandle: s => s.StartsWith("WARN"),
       handler: s => Console.WriteLine($"Warning logged: {s}")
   );

   registry.Register(
       canHandle: s => s.Contains("URGENT"),
       handler: s => SendAlert(s)
   );

   // Execute first matching handler
   registry.Handle("ERROR: Database connection failed"); // Logs error
   registry.Handle("URGENT WARN: System overload");      // Logs warning (first match)

   // Execute all matching handlers
   registry.HandleAll("URGENT WARN: System overload");   // Logs warning AND sends alert

Core Methods
------------

**Registration**

.. code-block:: csharp

   registry.Register(canHandle, handler);

**Execution**

.. code-block:: csharp

   // Execute first matching handler
   bool wasHandled = registry.Handle(item);

   // Execute all matching handlers
   int handlersExecuted = registry.HandleAll(item);

   // Check if any handler can process without executing
   bool canProcess = registry.CanHandle(item);

**Management**

.. code-block:: csharp

   // Get current handler count
   int count = registry.Count;

   // Remove all handlers and clear stash
   registry.Clear();

   // Get matching handlers without executing
   var handlers = registry.GetMatchingHandlers(item);

Stashing Handlers
-----------------

The stashing feature allows you to temporarily save the current handler set
and work with a different set of handlers:

.. code-block:: csharp

   var registry = new HandlerRegistry<string>();

   // Register production handlers
   registry.Register(s => s.StartsWith("INFO"), s => LogToFile(s));
   registry.Register(s => s.StartsWith("ERROR"), s => LogToDatabase(s));

   // Stash production handlers
   registry.Stash();

   // Register test handlers
   registry.Register(s => s.StartsWith("INFO"), s => LogToConsole(s));
   registry.Register(s => s.StartsWith("ERROR"), s => LogToConsole(s));

   // Run tests with console logging
   registry.Handle("INFO: Test message");

   // Restore production handlers
   registry.Pop();

   // Back to file/database logging
   registry.Handle("INFO: Production message");


The stash supports multiple levels - you can stash multiple times and pop them
back in LIFO (Last In, First Out) order.

Common Use Cases
----------------

**Message Processing**

.. code-block:: csharp

   var messageRegistry = new HandlerRegistry<Message>();

   messageRegistry.Register(
       canHandle: msg => msg.Type == MessageType.Command,
       handler: msg => commandProcessor.Process(msg)
   );

   messageRegistry.Register(
       canHandle: msg => msg.Type == MessageType.Event,
       handler: msg => eventStore.Save(msg)
   );

**HTTP Request Routing**

.. code-block:: csharp

   var routeRegistry = new HandlerRegistry<HttpRequest>();

   routeRegistry.Register(
       canHandle: req => req.Path.StartsWith("/api/users"),
       handler: req => userController.Handle(req)
   );

   routeRegistry.Register(
       canHandle: req => req.Path.StartsWith("/api/orders"),
       handler: req => orderController.Handle(req)
   );

**Validation Pipeline**

.. code-block:: csharp

   var validatorRegistry = new HandlerRegistry<Order>();

   validatorRegistry.Register(
       canHandle: order => order.Amount > 1000,
       handler: order => order.RequiresApproval = true
   );

   validatorRegistry.Register(
       canHandle: order => order.Customer.IsVip,
       handler: order => order.Priority = Priority.High
   );
