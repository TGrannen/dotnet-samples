using Dapper;
using IntegrationTesting.WebAPI.Infrastructure;

namespace IntegrationTesting.WebAPI.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsersController(IDbConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    /// <summary>
    /// WARNING: It is not best practice to do DB operations directly in API controllers. This is just to help illustrate integration test examples
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<User>> Get()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<User>("Select * from public.app_users");
    }
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}