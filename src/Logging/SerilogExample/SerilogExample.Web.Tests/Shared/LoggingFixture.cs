using Serilog;
using Serilog.Extensions.Logging;

namespace SerilogExample.Web.Tests.Shared;

public class LoggingFixture
{
    private readonly SerilogLoggerFactory _factory = new(new LoggerConfiguration().WriteTo.InMemory().CreateLogger());
    public ILogger<T> GetLogger<T>() => _factory.CreateLogger<T>();
    public InMemorySink Instance => InMemorySink.Instance;
}