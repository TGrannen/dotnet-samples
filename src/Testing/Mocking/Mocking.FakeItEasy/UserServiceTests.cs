using FakeItEasy;
using Microsoft.Extensions.Logging;
using Mocking.Shared;
using Shouldly;
using Xunit;

namespace Mocking.FakeItEasy;

public class UserServiceTests
{
    private readonly IUserService _service;
    private readonly IUserRepository _userRepository = A.Fake<IUserRepository>();
    private readonly ILogger<UserService> _logging = A.Fake<ILogger<UserService>>();

    public UserServiceTests()
    {
        _service = new UserService(_userRepository, _logging);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "Bob Barker";

        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>.Ignored)).Returns(new UserDto { Id = userId.ToString(), UserName = userName });

        // Act
        var user = await _service.GetByIdAsync(userId);

        // Assert
        user.ShouldNotBeNull();
        user.Id.ShouldBe(userId);
        user.UserName.ShouldBe(userName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>.Ignored)).Returns((UserDto)null);

        // Act
        var user = await _service.GetByIdAsync(Guid.NewGuid());

        // Assert
        user.ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogAppropriateMessage_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "Bob Barker";

        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>.Ignored)).Returns(new UserDto { Id = userId.ToString(), UserName = userName });

        // Act
        await _service.GetByIdAsync(userId);

        // Assert
        _logging.VerifyLogMustHaveHappened(LogLevel.Information, $"Retrieved a user with Id: {userId}");
        _logging.VerifyLogMustNotHaveHappened(LogLevel.Information, $"Unable to find a user with Id: {userId}");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogAppropriateMessage_WhenUserDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>.Ignored)).Returns((UserDto)null);

        // Act
        await _service.GetByIdAsync(userId);

        // Assert
        _logging.VerifyLogMustNotHaveHappened(LogLevel.Information, $"Retrieved a user with Id: {userId}");
        _logging.VerifyLogMustHaveHappened(LogLevel.Information, $"Unable to find a user with Id: {userId}");
    }
}