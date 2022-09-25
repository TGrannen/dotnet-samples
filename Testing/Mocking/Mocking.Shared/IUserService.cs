namespace Mocking.Shared;

public interface IUserService
{
    Task<List<User>> GetAllAsync();

    Task<User> CreateAsync(User user);

    Task<User> GetByIdAsync(Guid userId);
}