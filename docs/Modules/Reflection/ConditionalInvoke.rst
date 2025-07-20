Conditional Invoke
==================

The ``InterfaceInvokeExtensions`` class provides extension methods for
conditionally executing actions or functions based on whether an object
implements a specific interface or inherits from a particular type.

These extension methods implement the "try-cast and invoke" pattern, allowing
you to safely execute code only when an object supports the required interface
or type. This eliminates the need for manual type checking and casting, making
code more readable and less error-prone.

**Key Features:**

* **Type-Safe Execution**: Only executes actions when the object implements the
  target interface
* **Null Safety**: Gracefully handles null objects without throwing exceptions
* **Async Support**: Provides async variants for asynchronous operations
* **Return Value Support**: Can execute functions and return results
  conditionally
* **Generic Flexibility**: Works with any interface, abstract class, or
  concrete type

**Available Methods**

.. code-block:: csharp

   public static void InvokeIf<TTarget>(this object entity, Action<TTarget> action)
       where TTarget : class

.. code-block:: csharp

   public static async Task InvokeIfAsync<TTarget>(this object entity, Func<TTarget, Task> action)
       where TTarget : class

.. code-block:: csharp

   public static async Task<TResult?> InvokeIfAsync<TTarget, TResult>(this object entity, Func<TTarget, Task<TResult>> action)
       where TTarget : class

.. code-block:: csharp

   public static TResult? InvokeIf<TTarget, TResult>(this object entity, Func<TTarget, TResult> action)
       where TTarget : class

Conditionally executes an action if the object implements the specified
interface or type.

**Example:**

.. code-block:: csharp

   object someObject = new FileStream("file.txt", FileMode.Open);

   // Works with any interface or class
   someObject.InvokeIf<Stream>(stream =>
   {
       Console.WriteLine($"Stream length: {stream.Length}");
       stream.Seek(0, SeekOrigin.Begin);
   });

   // Only executes if someObject implements IDisposable
   someObject.InvokeIf<IDisposable>(disposable => disposable.Dispose());
