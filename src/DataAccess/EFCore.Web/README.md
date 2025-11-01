# Entity Framework Core

Entity Framework (EF) Core is a lightweight, extensible, open source and cross-platform version of the popular Entity Framework data access technology.

EF Core can serve as an object-relational mapper (O/RM), which:

* Enables .NET developers to work with a database using .NET objects.
* Eliminates the need for most of the data-access code that typically needs to be written.

This example project shows how to use EF Core to access and store data. It also showcase how to use automated code first schema Migrations. Schema model was taken from this [Sample](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-5.0). Migration infromation can be found [here](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli).

## Migrations commands run in this example project

### First Migration

``` bash
dotnet ef migrations add InitialCreate -o Persistence/Migrations
```

### Second Migration

``` bash
dotnet ef migrations add UpdateDataModel -o Persistence/Migrations
```
