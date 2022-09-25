namespace Mocking.Moq.Loggers;

internal class MockedLoggerFactory
{
    private readonly Dictionary<string, IMockedLogger> _loggers = new Dictionary<string, IMockedLogger>();

    public MockedILogger<T> GetLogger<T>()
    {
        var fullName = typeof(T).FullName ?? throw new TypeAccessException("Couldn't get Generic type name");

        if (!_loggers.ContainsKey(fullName))
        {
            var mockLogger = new MockedILogger<T>();
            _loggers.Add(fullName, mockLogger);
        }

        return _loggers[fullName] as MockedILogger<T>;
    }

    public IEnumerable<IMockedLogger> GetMockedLoggers()
    {
        return _loggers.Values;
    }
}