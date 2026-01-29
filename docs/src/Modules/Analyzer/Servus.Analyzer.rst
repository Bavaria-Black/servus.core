Servus.Analyzer
================

As of **Servus.Core v0.0.0** we now include `Servus.Analyzers`
as a package dependency for the core Servus library,
which means any projects that reference anything depending
on `Servus.Core` will automatically pull in all
Servus.Analyzer's rules and code fixes.

Servus.Analyzer is a `Roslyn Analysis and Code Fix <https://learn.microsoft.com/en-us/visualstudio/extensibility/getting-started-with-roslyn-analyzers>`__
package, which leverages the .NET compiler platform ("Roslyn")
to detect Servus-specific anti-patterns during compilation.

Supported Rules
---------------

.. list-table::
   :header-rows: 1
   :widths: 15 60 10 20

   * - Id
     - Title
     - Severity
     - Category
   * - `SUS1000 <Modules/Analyzer/Rules/SUS1000>`_
     - Avoid using ``TraceId``
     - Warning
     - Trace Usage
   * - `SUS1001 <Modules/Analyzer/Rules/SUS1000>`_
     - Avoid using ``SpanId``
     - Warning
     - Trace Usage
   * - `SUS1002 <Modules/Analyzer/Rules/SUS1002>`_
     - Avoid obsolete ``AddTracing(traceId, spanId)`` overload
     - Error
     - Trace Usage
