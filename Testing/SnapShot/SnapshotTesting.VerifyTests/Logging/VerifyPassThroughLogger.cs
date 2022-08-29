using Microsoft.Extensions.Logging;

namespace SnapshotTesting.VerifyTests.Logging;

class VerifyPassThroughLogger<T> : ILogger<T>
{
    private readonly ILogger<T> _logger;

    public VerifyPassThroughLogger(LoggerProvider loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }
}