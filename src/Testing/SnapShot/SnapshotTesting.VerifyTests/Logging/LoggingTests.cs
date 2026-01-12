using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VerifyTests.MicrosoftLogging;

namespace SnapshotTesting.VerifyTests.Logging;

public class LoggingTests
{
    [Fact]
    public Task Logging()
    {
        Recording.Start();
        var provider = new RecordingLogger();
        ClassThatUsesLogging target = new(provider);

        var result = target.Method();

        return Verify(result);
    }

    [Fact]
    public Task LoggingTyped()
    {
        Recording.Start();

        var logger = new RecordingProvider().CreateLogger<ClassThatUsesTypedLogging>();
        ClassThatUsesTypedLogging target = new(logger);

        var result = target.Method();

        return Verify(result);
    }

    [Fact]
    public async Task LoggingTyped_ViaServiceDependencyInjection()
    {
        Recording.Start();
        var collection = new ServiceCollection();
        var logger = new RecordingProvider().CreateLogger<ClassThatUsesTypedLogging>();
        collection.AddSingleton(logger);
        collection.AddScoped<ClassThatUsesTypedLogging>();
        await using var provider = collection.BuildServiceProvider();
        var myService = provider.GetRequiredService<ClassThatUsesTypedLogging>();

        var result = myService.Method();

        await Verify(result).IgnoreMembers("Date");
    }

    private class ClassThatUsesLogging(ILogger logger)
    {
        public string Method()
        {
            logger.LogWarning("The log entry");
            using (logger.BeginScope("The scope"))
            {
                logger.LogWarning("Entry in scope");
            }

            return "result";
        }
    }

    private class ClassThatUsesTypedLogging(ILogger<ClassThatUsesTypedLogging> logger)
    {
        public string Method()
        {
            logger.LogWarning("The log entry with value {Value}", 45694);
            return "result";
        }
    }
}