namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests;

public class UsersAPIIntegrationTests : DbRespawnTests
{
    private IUsersAPI _api;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _api = new AppFactoryWithDb<Program>().CreateRefitClient<Program, IUsersAPI>();
    }

    [Test]
    public async Task GetUsers_ShouldReturnAnEmptyCollection()
    {
        var users = await _api.GetUsers();
        users.ShouldBeEmpty();
    }

    [Test]
    public async Task CreateUser_ShouldReturnAGuid_ThatCanBeUsedToGetUser()
    {
        var dto = new UserDto
        {
            Email = "test@test.com",
            Name = "Fred Wow"
        };
        var id = await _api.CreateUser(dto);
        id.ShouldNotBe(Guid.Empty);

        var user = await _api.GetUser(id);
        user.ShouldNotBeNull();
        user.Id.ShouldBe(id);
        user.Name.ShouldBe(dto.Name);
        user.Email.ShouldBe(dto.Email);
    }

    [Test]
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
}