CircularQueue
=============

A generic circular queue implementation that automatically removes the oldest
items when capacity is exceeded. Perfect for scenarios like logging, caching,
or maintaining a fixed-size history of items.

Usage
-----

.. code-block:: csharp

    // Create a circular queue with capacity of 3
    var buffer = new CircularQueue<string>(3);

    // Add items
    buffer.Enqueue("A");
    buffer.Enqueue("B");
    buffer.Enqueue("C");
    // Queue: [A, B, C]

    // Adding another item removes the oldest
    buffer.Enqueue("D");
    // Queue: [B, C, D] - "A" was automatically removed

    // Dequeue items in FIFO order
    if (buffer.TryDequeue(out string item))
    {
        Console.WriteLine(item); // Prints "B"
    }
