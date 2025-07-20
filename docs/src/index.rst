.. Servus.Core documentation master file, created by
   sphinx-quickstart on Sun Jul 20 12:06:04 2025.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

Servus to the Core documentation!
=================================

.. image:: _static/logo.png
   :align: center

**Servus.Core** was born out of frustration - the kind every
developer knows when you find yourself writing the same
helper methods over and over again. This library is a
collection of those "I wish this was built into .NET"
utilities that somehow always end up missing from your toolkit.

Whether it's string manipulation that actually makes sense,
date handling that doesn't make you cry, or collection
operations that just work the way you expect them to -
**Servus.Core** has you covered. It's the Swiss Army knife for
.NET developers who are tired of reinventing the wheel.


.. toctree::
   :maxdepth: 2
   :caption: Application:

   Modules/Startup/main
   Modules/AwaitableConditions/index
   Modules/Diagnostics/tracing

.. toctree::
   :maxdepth: 2
   :caption: Extensions:

   Modules/Reflection/ConditionalInvoke
