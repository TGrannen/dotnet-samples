using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mocking.Moq.Extensions;
using Mocking.Moq.Loggers;
using Mocking.Shared;
using Moq;
using Xunit;

namespace Mocking.Moq;

public class UserServiceTests
{
    private readonly UserService _service;
    private readonly Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
    private readonly MockedILogger<UserService> _loggerMock = new MockedILogger<UserService>();

    public UserServiceTests()
    {
        _service = new UserService(_userRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "Bob Barker";

        _userRepoMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new UserDto
            {
                Id = userId.ToString(),
                UserName = userName
            });

        // Act
        var user = await _service.GetByIdAsync(userId);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(userId);
        user.UserName.Should().Be(userName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNothing_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

        // Act
        var user = await _service.GetByIdAsync(Guid.NewGuid());

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogAppropriateMessage_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new UserDto
            {
                Id = userId.ToString(),
                UserName = "Bob Barker"
            });

        // Act
        await _service.GetByIdAsync(userId);

        // Assert
        _loggerMock
            .VerifyLogging(LogLevel.Information, $"Retrieved a user with Id: {userId}", Times.Once)
            .VerifyLogging(LogLevel.Information, $"Unable to find a user with Id: {userId}", Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogAppropriateMessage_WhenUserDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _service.GetByIdAsync(userId);

        // Assert
        _loggerMock
            .VerifyLogging(LogLevel.Information, $"Retrieved a user with Id: {userId}", Times.Never)
            .VerifyLogging(LogLevel.Information, $"Unable to find a user with Id: {userId}", Times.Once);
    }
}