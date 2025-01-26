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
    public async Task<IActionResult> Get()
    {
        using var conn = _connectionFactory.CreateConnection();
        return Ok(await conn.QueryAsync<User>("Select * from public.app_users"));
    }

    /// <summary>
    /// WARNING: It is not best practice to do DB operations directly in API controllers. This is just to help illustrate integration test examples
    /// </summary>
    /// <returns></returns>
    [HttpGet("/users/{id}", Name = "GetUser")]
    public async Task<IActionResult> Get(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        var user = await conn.QueryFirstOrDefaultAsync<User>("Select * from public.app_users where id = @Id", new
        {
            Id = id
        });
        return Ok(user);
    }

    /// <summary>
    /// WARNING: It is not best practice to do DB operations directly in API controllers. This is just to help illustrate integration test examples
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> Create([FromBody] CreateUser user)
    {
        using var conn = _connectionFactory.CreateConnection();
        var sql = "INSERT INTO app_users (id, Name, email) VALUES (@id, @name, @email) RETURNING id";

        var id = await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            id = Guid.NewGuid(),
            name = user.Name,
            email = user.Email
        });

        return Ok(id);
    }
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUser
{
    public string Name { get; set; }
    public string Email { get; set; }
}