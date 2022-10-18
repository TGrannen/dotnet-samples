using IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests;

[Collection(nameof(AppFactoryWithSeededDbCollection))]
public class SeededUsersAPIIntegrationTests
{
    private readonly IUsersAPI _api;

    public SeededUsersAPIIntegrationTests(AppFactoryWithSeededDb<Program> factory)
    {
        _api = factory.CreateRefitClient<IUsersAPI>();
    }

    [Fact]
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
}