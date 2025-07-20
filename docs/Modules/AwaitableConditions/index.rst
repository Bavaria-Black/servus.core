AwaitableCondition
==================

The ``AwaitableCondition`` abstract class provides a foundation for creating
asynchronous conditions that can be awaited until they are met or timeout
occurs.

Overview
--------

``AwaitableCondition`` allows you to create custom conditions that can be
awaited asynchronously. This is particularly useful for scenarios where you
need to wait for external events, state changes, or complex conditions to be
satisfied before proceeding.

**Key Features:**

* **Asynchronous Waiting**: Wait for conditions using ``async/await`` patterns
* **Timeout Support**: Built-in timeout handling with configurable behavior
* **Cancellation Support**: Integrates with ``CancellationToken`` for
  cooperative cancellation
* **Extensible**: Override abstract and virtual methods to implement custom
  logic
* **Exception Control**: Choose whether timeouts throw exceptions or return
  ``false``

Basic Usage
------------

.. code-block:: csharp

   public Task<bool> WaitAsync()

Waits asynchronously for the condition to be satisfied. This returns ``true``
if condition is met and the internal condition changed. It could also return
``false`` when the internal condition changed. If
``throwOnExceptionIfCanceled`` was set, it will throw an
OperationCanceledException instead of return ``false``.


**Example:**

.. code-block:: csharp

   var condition = new DatabaseConnectionCondition("Server=localhost;...", 30000);

   try
   {
       bool connected = await condition.WaitAsync();
       if (connected)
       {
           Console.WriteLine("Database is ready!");
       }
   }
   catch (OperationCanceledException)
   {
       Console.WriteLine("Timeout waiting for database connection");
   }

Implementation
--------------

.. code-block:: csharp

   protected abstract bool Evaluate()

**Must be implemented by derived classes.** This method contains the logic
to check if the condition is satisfied. Should return ``true`` if the condition
is met, ``false`` otherwise.

This Method will be called indirectly by ``OnConditionChanged```. Call this for
every event that could potentially changing the outcome of ``Evaluate``.

**Implementation Guidelines:**

* Keep evaluation logic fast and non-blocking
* Avoid throwing exceptions; return ``false`` for failed conditions

**Example:**

.. code-block:: csharp

   protected override bool Evaluate()
   {
       try
       {
           using var client = new HttpClient();
           var response = client.GetAsync(apiEndpoint).Result;
           return response.IsSuccessStatusCode;
       }
       catch
       {
           return false; // Don't throw, just return false
       }
   }

Virtual Methods (State changes)
-------------------------------

* **OnSuccess** - This will be called when the condition is successfully met.
* **OnFailed** - This will be called when condition evaluation returns
  ``false``.
* **OnCanceled** - This will be called when the condition is canceled due to
  timeout or cancellation token.

**Example:**

.. code-block:: csharp

   protected override void OnCanceled()
   {
       logger.LogWarning("Condition wait was canceled after timeout");
   }

Complete Examples
-----------------

File System Watcher Condition
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   public class FileCreatedCondition : AwaitableCondition
   {
       private readonly string filePath;
       private readonly FileSystemWatcher watcher;

       public FileCreatedCondition(string filePath, CancellationToken cancellationToken)
           : base(cancellationToken, throwExceptionIfCanceled: false)
       {
           this.filePath = filePath;

           var directory = Path.GetDirectoryName(filePath);
           var fileName = Path.GetFileName(filePath);

           watcher = new FileSystemWatcher(directory, fileName);
           watcher.Created += (s, e) => OnConditionChanged();
           watcher.EnableRaisingEvents = true;
       }

       protected override bool Evaluate()
       {
           return File.Exists(filePath);
       }

       protected override void OnSuccess()
       {
           watcher?.Dispose();
       }

       protected override void OnCanceled()
       {
           watcher?.Dispose();
       }
   }

   // Usage
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
   var fileCondition = new FileCreatedCondition(@"C:\temp\expected-file.txt", cts.Token);
   bool fileCreated = await fileCondition.WaitAsync();

HTTP Service Readiness Condition
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   public class ServiceReadinessCondition : AwaitableCondition
   {
       private readonly HttpClient httpClient;
       private readonly string healthCheckUrl;
       private readonly Timer pollTimer;

       public ServiceReadinessCondition(string healthCheckUrl, TimeSpan pollInterval, int timeoutMs)
           : base(timeoutMs)
       {
           this.healthCheckUrl = healthCheckUrl;
           this.httpClient = new HttpClient();

           // Poll every interval
           this.pollTimer = new Timer(_ => OnConditionChanged(), null, TimeSpan.Zero, pollInterval);
       }

       protected override bool Evaluate()
       {
           try
           {
               var response = httpClient.GetAsync(healthCheckUrl).Result;
               return response.IsSuccessStatusCode;
           }
           catch
           {
               return false;
           }
       }

       protected override void OnSuccess()
       {
           pollTimer?.Dispose();
           httpClient?.Dispose();
       }

       protected override void OnCanceled()
       {
           pollTimer?.Dispose();
           httpClient?.Dispose();
       }
   }

   // Usage
   var serviceCondition = new ServiceReadinessCondition(
       "https://api.myservice.com/health",
       TimeSpan.FromSeconds(2),
       30000);

   bool isReady = await serviceCondition.WaitAsync();

Common Use Cases
----------------

* **Service Startup**: Wait for databases, APIs, or external services to become
  available
* **File Operations**: Wait for files to be created, modified, or become
  accessible
* **Resource Availability**: Wait for network connections, device availability,
  or system resources
* **State Synchronization**: Wait for application state changes or business
  conditions
* **Testing**: Create predictable delays and conditions in unit and integration
  tests
