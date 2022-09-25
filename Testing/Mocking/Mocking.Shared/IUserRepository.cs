namespace Mocking.Shared;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto> GetByIdAsync(Guid id);

    Task<UserDto> CreateAsync(UserDto user);
}