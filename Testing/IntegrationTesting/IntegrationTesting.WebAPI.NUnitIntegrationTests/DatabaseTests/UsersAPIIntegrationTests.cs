namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests;

public class UsersAPIIntegrationTests
{
    private IUsersAPI _api;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _api = new AppFactoryWithDb<Program>().CreateRefitClient<Program,IUsersAPI>();
    }

    [Test]
    public async Task GetUsers_ShouldReturnANonEmptyCollection()
    {
        var users = await _api.GetUsers();
        users.Should().NotBeEmpty();
        users.Should().AllSatisfy(x =>
        {
            x.Id.Should().NotBeEmpty();
            x.Email.Should().NotBeEmpty();
            x.Name.Should().NotBeEmpty();
        });
    }
}

internal interface IUsersAPI
{
    [Get("/Users")]
    Task<List<UserDto>> GetUsers();
}

public class UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}