# Dapper

Dapper is a popular simple object mapping tool. It is designed primarily to be used in scenarios where you want to work
with data in a strongly typed fashion - as business objects in a .NET application, but don't want to spend hours writing
code to map query results from ADO.NET data readers to instances of those objects.

This project showcases the ability to use [Dapper](https://github.com/DapperLib/Dapper) to query a Postgres database
and return C# POCO objects with their properties populated by the database values. It will spin up a postgres container
using the [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) package to not have to depend
external setup for the database.

#### DB Example Source

https://github.com/vrajmohan/pgsql-sample-data/tree/master/employee

#### Packages include

* [Dapper](https://github.com/DapperLib/Dapper)
* [Npgsql](https://www.npgsql.org/)
* [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)
