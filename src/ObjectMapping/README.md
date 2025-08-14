# AutoMapper

[AutoMapper](https://docs.automapper.org/en/stable/index.html) is a package used to map data from one object to another.

AutoMapper uses a convention-based matching algorithm to match up source to destination values. It's geared towards model projection scenarios to flatten complex object models to
DTOs and other simple objects, whose design is better suited for serialization, communication, messaging, or simply an anti-corruption layer between the domain and application
layer.

---

# Mapster

[Mapster](https://github.com/MapsterMapper/Mapster) is a lightweight, high-performance object-to-object mapper for .NET. It supports runtime mapping, fluent configuration, and
optional source generation for compile-time mappings.

- Docs: https://github.com/MapsterMapper/Mapster/wiki

---

# Facet

[Facet](https://github.com/Tim-Maes/Facet) is a lightweight, composable object-to-object mapping library for .NET. It focuses on clear, strongly-typed mappings and efficient
projections to DTOs or view models.

## Installation

```
bash
dotnet add package Facet
```

## Overview

- Enables mapping between domain models and DTOs/view models.
- Supports custom member mappings and projections.
- Designed to be easy to adopt alongside or instead of other mappers.

## Getting started

- Install the package and import the Facet namespaces in your mapping code.
- Define mappings to project your source types to destination types.
- Refer to the repository for examples and detailed usage.

- Repository and docs: https://github.com/Tim-Maes/Facet
