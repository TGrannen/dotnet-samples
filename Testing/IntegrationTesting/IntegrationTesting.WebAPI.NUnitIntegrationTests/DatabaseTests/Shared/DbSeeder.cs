using Bogus;
using Dapper;
using Npgsql;

namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests;

public static class DbSeeder
{
    public static async Task CreateSchema(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteScalarAsync(CreateTable);
    }

    public static async Task SeedData(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        var faker = new Faker<UserDto>()
                .RuleFor(x => x.Name, f => f.Person.FullName)
                .RuleFor(x => x.Email, f => f.Person.Email)
            ;
        foreach (var dto in faker.Generate(5))
        {
            await CreateUser(conn, dto);
        }
    }

    private static async Task CreateUser(NpgsqlConnection connection, UserDto userDto)
    {
        var sql = "INSERT INTO app_users (id, Name, email) VALUES (@id, @name, @email)";

        await connection.ExecuteAsync(sql, new
        {
            id = Guid.NewGuid(),
            name = userDto.Name,
            email = userDto.Email
        });
    }

    private const string CreateTable = @"
        CREATE TABLE IF NOT EXISTS public.app_users (
          id uuid NOT NULL,
          name varchar(45) NOT NULL,
          email varchar(450) NOT NULL,
          PRIMARY KEY (id)
        )";
}