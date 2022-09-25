using Microsoft.Extensions.Logging;

namespace Mocking.Moq.Loggers;

internal class MockedPassThroughLogger<T> : ILogger<T>
{
    private readonly MockedILogger<T> _mockedILogger;

    public MockedPassThroughLogger(MockedLoggerFactory factory)
    {
        _mockedILogger = factory.GetLogger<T>();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        _mockedILogger.Object.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _mockedILogger.Object.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _mockedILogger.Object.BeginScope(state);
    }
}