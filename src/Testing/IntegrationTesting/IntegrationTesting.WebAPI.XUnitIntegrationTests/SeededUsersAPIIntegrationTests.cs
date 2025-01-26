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
        users.ShouldNotBeEmpty();
        foreach (var user in users)
        {
            user.ShouldSatisfyAllConditions(
                () => user.Id.ShouldNotBe(Guid.Empty),
                () => user.Email.ShouldNotBeNullOrEmpty(),
                () => user.Name.ShouldNotBeNullOrEmpty()
            );
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
}