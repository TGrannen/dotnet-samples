# Dapper - Clean Architecture Example

To help with using Dapper in the Clean Architecture project structure, there are two interfaces provided in this example
that can be used to simplify connection and transaction management in MediatR requests within the Application project.

* `IDbReadContext` - Injects a DbConnection to be used in Read Only scenarios
* `IDbContext` - Injects a DbConnection within an open transaction to allow grouping of data altering requests and Domain event publishing

The main web project will spin up a postgres container
using the [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) package to not have to depend
external setup for the database.

### Context Interfaces

```csharp
public interface IDbContext
{
    /// <summary>
    /// Connection tied to a transaction that is not committed until SaveChangesAsync is called
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Add events to a temporary collection to be fired once all changes have been successfully saved
    /// </summary>
    void AddEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Commit in progress Database transaction and fire any pending events after successful commit
    /// </summary>
    Task SaveChangesAsync(CancellationToken token = default);
}
```

```csharp
public interface IDbReadContext
{
    public IDbConnection Connection { get; }
}
```

#### DB Example Source

https://github.com/vrajmohan/pgsql-sample-data/tree/master/employee

#### Packages include

* [Dapper](https://github.com/DapperLib/Dapper)
* [Npgsql](https://www.npgsql.org/)
* [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)
* [MediatR](https://github.com/jbogard/MediatR)
* [Respawn](https://github.com/jbogard/Respawn)
* [Moq](https://github.com/moq/moq)
* [Shouldly](https://docs.shouldly.org/)
