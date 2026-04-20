# Application

Everything you need to bootstrap a .NET application with clean, testable startup code. The `Application` layer covers fluent app building, modular setup containers, dependency gating, health checks, and a couple of runtime conveniences like the colored console helper and the Servus brand constants.

## Pages in this section

- [**AppBuilder**](./app-builder) — fluent entry point that wires host, configuration, services, logging, and lifecycle hooks into one `RunAsync`.
- [**Setup Containers**](./setup-containers) — the modular building blocks `AppBuilder` composes; split your DI wiring into small, reusable pieces.
- [**Startup Gates**](./startup-gates) — block app startup until external dependencies (DB, APIs, licenses) are actually ready.
- [**Health Checks**](./health-checks) — drop-in base class for wiring ASP.NET Core health checks and a `/healthz` endpoint.
- [**Servus Environment**](./environment) — environment-variable helpers (`SERVUS_*` prefix) and ASCII logo constants.
- [**Console Utilities**](./console) — colored output, key/value printers, and a helper to redirect a spawned process's output into your own console.

## Namespace map

| Namespace | Location |
|---|---|
| `Servus.Core.Application` | Root app helpers (logo, env) |
| `Servus.Core.Application.Startup` | `AppBuilder`, `AppRunner`, setup container contracts |
| `Servus.Core.Application.Startup.Gates` | `IStartupGate` + `ISetupStartupGates` |
| `Servus.Core.Application.HealthChecks` | `HealthCheckSetupContainer` |
| `Servus.Core.Application.Console` | `ServusConsole`, `ProcessOutRedirector` |
