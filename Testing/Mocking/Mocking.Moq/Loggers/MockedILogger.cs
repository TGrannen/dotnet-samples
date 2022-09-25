using Microsoft.Extensions.Logging;
using Moq;

namespace Mocking.Moq.Loggers;

public class MockedILogger<T> : Mock<ILogger<T>>, IMockedLogger
{
}