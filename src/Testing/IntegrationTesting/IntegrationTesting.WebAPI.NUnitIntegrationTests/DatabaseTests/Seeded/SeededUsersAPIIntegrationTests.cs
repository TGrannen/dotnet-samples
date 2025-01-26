namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests.Seeded;

public class SeededUsersAPIIntegrationTests
{
    private IUsersAPI _api;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _api = new AppFactoryWithSeededDb<Program>().CreateRefitClient<Program, IUsersAPI>();
    }

    [Test]
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