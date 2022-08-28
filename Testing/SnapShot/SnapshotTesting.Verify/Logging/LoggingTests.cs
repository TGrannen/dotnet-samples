using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SnapshotTesting.Verify.Logging;

[Collection("Logging Collection")]
[UsesVerify]
public class LoggingTests
{
    [Fact]
    public Task Logging()
    {
        var provider = LoggerRecording.Start();
        ClassThatUsesLogging target = new(provider);

        var result = target.Method();

        return Verifier.Verify(result);
    }

    [Fact]
    public Task LoggingTyped()
    {
        var provider = LoggerRecording.Start();
        var logger = provider.CreateLogger<ClassThatUsesTypedLogging>();
        ClassThatUsesTypedLogging target = new(logger);

        var result = target.Method();

        return Verifier.Verify(result);
    }

    class ClassThatUsesLogging
    {
        private readonly ILogger _logger;

        public ClassThatUsesLogging(ILogger logger)
        {
            _logger = logger;
        }

        public string Method()
        {
            _logger.LogWarning("The log entry");
            using (_logger.BeginScope("The scope"))
            {
                _logger.LogWarning("Entry in scope");
            }

            return "result";
        }
    }

    class ClassThatUsesTypedLogging
    {
        private readonly ILogger<ClassThatUsesTypedLogging> _logger;

        public ClassThatUsesTypedLogging(ILogger<ClassThatUsesTypedLogging> logger)
        {
            _logger = logger;
        }

        public string Method()
        {
            _logger.LogWarning("The log entry with value {Value}", 45694);
            return "result";
        }
    }
}