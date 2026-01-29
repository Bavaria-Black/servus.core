ActivitySourceRegistry
======================

Static utility for managing ``ActivitySource`` instances with type-based
registration and automatic snake_case naming. Works with ``IWithTracing``
messages and events to maintain distributed tracing context.

The ``ActivitySourceRegistry.StartActivity<T>`` creates and starts an activity
using the ActivitySource for type ``T`` and the tracing context from the
message/event.

Basic usage
-----------

.. code-block:: csharp

    public class ProcessOrderMessage : IWithTracing
    {
        public string? TraceParent { get; set; }
        public string? TraceState { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderService
    {
        public async Task HandleOrderMessage(ProcessOrderMessage message)
        {
            // AddTracing should have been called before sending the message
            using var activity = ActivitySourceRegistry.StartActivity<OrderService>(
                "process-order", message);

            // Process the order
            await ProcessOrderAsync(message.OrderId);
        }

        public async Task SendOrderMessage(int orderId, decimal amount)
        {
            var message = new ProcessOrderMessage
            {
                OrderId = orderId,
                Amount = amount
            };

            // Always call AddTracing before sending
            ((IWithTracing)message).AddTracing();

            await messageQueue.SendAsync(message);
        }
    }


ActivitySourceNameAttribute
---------------------------

The ``ActivitySourceNameAttribute`` can be applied to classes to provide a
consistent ActivitySource name without requiring explicit name parameters in
method calls. This attribute eliminates the need to repeatedly specify the
same source name across related operations.

.. code-block:: csharp

  [ActivitySourceName("MyCompany.OrderProcessing")]
  public class OrderService
  {
      // ActivitySource name is automatically resolved from the attribute
  }


ActivitySourceKeyAttribute
--------------------------

Use ``ActivitySourceKey`` to designate another class as the primary
ActivitySource in the registry. This pattern allows you to centralize
ActivitySource management in a base class while reducing the total number
of ActivitySources needed across your application.

.. code-block:: csharp

  // This class becomes the root ActivitySource for derived classes
  [ActivitySourceKey(typeof(OrderService))]
  public class SubOrderProcess
  {
      public void DoWork()
      {
          // this will resolve the ActivitySource for OrderService
          using var activity = ActivitySourceRegistry.StartActivity<SubOrderProcess>(
              "do-work", message);
      }
  }

  [ActivitySourceName("MyCompany.OrderProcessing")]
  public class OrderService
  {
      // This is the main key that is used for the key to resolve the
      // corresponding ActivitySource
  }

This approach promotes ActivitySource reuse and reduces configuration overhead
while maintaining clean separation of concerns.
