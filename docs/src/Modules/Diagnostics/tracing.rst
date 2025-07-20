ActivitySourceRegistry
======================

Static utility for managing ``ActivitySource`` instances with type-based
registration and automatic snake_case naming. Works with ``IWithTracing``
messages and events to maintain distributed tracing context.


StartActivity
~~~~~~~~~~~~~

The ``ActivitySourceRegistry.StartActivity<T>`` creates and starts an activity using the
ActivitySource for type ``T`` and the tracing context from the message/event.

Usage
-----

.. code-block:: csharp

    public class ProcessOrderMessage : IWithTracing
    {
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }
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

Naming
------

Type names are automatically converted to snake_case:

* ``OrderService`` → ``order_service``
* ``PaymentProcessor`` → ``payment_processor``

Best Practices
--------------

1. Always call ``((IWithTracing)message).AddTracing()`` before sending messages/events
2. Use descriptive, kebab-case activity names: ``"process-payment"``
3. Use ``using`` statements for proper disposal
4. Add relevant tags: ``activity?.SetTag("order.id", orderId)``