using IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests;

[Collection(nameof(AppFactoryWithDbCollection))]
public class UsersAPIIntegrationTests : DbTestsBase<Program>
{
    private readonly IUsersAPI _api;

    public UsersAPIIntegrationTests(AppFactoryWithDb<Program> factory) : base(factory)
    {
        _api = factory.CreateRefitClient<IUsersAPI>();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnANonEmptyCollection()
    {
        var users = await _api.GetUsers();
        users.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnAGuid_ThatCanBeUsedToGetUser()
    {
        var id = await _api.CreateUser(new UserDto
        {
            Email = "test@test.com",
            Name = "Fred Wow"
        });
        id.ShouldNotBe(Guid.Empty);

        var user = await _api.GetUser(id);
        user.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateUser_ShouldAddOneUserToAllUsers()
    {
        var users = await _api.GetUsers();
        users.ShouldBeEmpty();

        await _api.CreateUser(new UserDto
        {
            Email = "test@test.com",
            Name = "Fred Wow"
        });

        users = await _api.GetUsers();
        users.Count.ShouldBe(1);
    }
}

internal interface IUsersAPI
{
    [Get("/Users")]
    Task<List<UserDto>> GetUsers();

    [Get("/Users/{id}")]
    Task<UserDto> GetUser(Guid id);

    [Post("/Users")]
    Task<Guid> CreateUser(UserDto userDto);
}

public class UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}