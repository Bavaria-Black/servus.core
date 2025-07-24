# Servus.Core

> A general-purpose .NET utility library. Filled with features that you didn't know you actually missed ;)

[![NuGet](https://img.shields.io/nuget/v/Servus.Core.svg)](https://www.nuget.org/packages/Servus.Core/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/Bavaria-Black/servus.core/build-and-release.yml?branch=develop)](https://github.com/Bavaria-Black/servus.core/actions)
[![License](https://img.shields.io/github/license/Bavaria-Black/servus.core)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/Servus.Core.svg)](https://www.nuget.org/packages/Servus.Core/)

<p align="center">
     <img width="128" height="128" src="docs/src/_static/logo.png" alt="servus.akka logo">
</p>

## Overview

**Servus.Core** was born out of frustration - the kind every developer knows when you find yourself writing the same helper methods over and over again. This library is a collection of those "I wish this was built into .NET" utilities that somehow always end up missing from your toolkit.

Whether it's string manipulation that actually makes sense, date handling that doesn't make you cry, or collection operations that just work the way you expect them to - Servus.Core has you covered. It's the Swiss Army knife for .NET developers who are tired of reinventing the wheel.

## Why Servus.Core?

- **Born from Real Projects**: Every utility comes from actual production use cases, not theoretical scenarios
- **No more Copy-Paste Programming**: Stop copying helper methods between projects
- **Just Works**: Simple utilities that do exactly what you expect without surprises

## Whats new?

Here are a few new features and changes that were done in this library. This is not specifically pinned to Versions.

 - Started to add documentation over at [Read the docs](https://servuscore.readthedocs.io/en/latest/)
 - Added Healcheck SetupContainer
 - AppBuilder for clean application startup
 - More casing functions
 - **MOVED** casing function to _Servus.Core.Text_
 - Added ActivitySourceRegistry
 - Added CircularQueue
 - Added HandlerRegistry
 - Added some Extension Methods for lists and types

## Installation

### Package Manager
```
Install-Package Servus.Core
```

### .NET CLI
```bash
dotnet add package Servus.Core
```

### PackageReference
```xml
<PackageReference Include="Servus.Core" Version="1.0.0" />
```

## Contributing

Contributions are welcome! This library grows with the community's needs.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-utility`
3. **Write** tests for your changes
4. **Ensure** all tests pass: `dotnet test`
5. **Submit** a Pull Request

### Contribution Guidelines

- Follow existing code style and conventions
- Include unit tests for new features
- Keep changes focused and atomic

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Servus and happy coding!** 🥨

*Feel free to use it and feel free to contribute other useful stuff.*

For questions or support, please [open an issue](https://github.com/Bavaria-Black/servus.core/issues) and/or [read the docs](https://servuscore.readthedocs.io/en/latest/).