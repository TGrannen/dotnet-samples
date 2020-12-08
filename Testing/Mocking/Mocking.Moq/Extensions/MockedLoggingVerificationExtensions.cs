using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;

namespace Mocking.Moq.Extensions
{
    /// <summary>
    /// Expanded upon the blog post <see href="https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq">Here</see>
    /// </summary>
    public static class MockedLoggingVerificationExtensions
    {
        public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> logger,
            LogLevel expectedLogLevel,
            string expectedMessage,
            Times? times,
            string failMessage = null)
        {
            times ??= Times.Once();
            failMessage ??= $"Logging verification failed for message: '{expectedMessage}'. {GetVerifyCountFailMessage(expectedLogLevel, (Times)times)}";

            logger.VerifyLogging(expectedLogLevel, (v, t) => v.ToString().CompareTo(expectedMessage) == 0, (Times)times, failMessage);

            return logger;
        }

        public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> logger,
            LogLevel expectedLogLevel,
            string expectedMessage,
            Func<Times> times,
            string failMessage = null)
        {
            times ??= Times.Once;

            return logger.VerifyLogging(expectedLogLevel, expectedMessage, times(), failMessage);
        }

        public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> logger,
            LogLevel expectedLogLevel,
            Func<Times> times,
            string failMessage = null)
        {
            var timeCount = times?.Invoke() ?? Times.Once();

            failMessage ??= $"Logging verification failed for evaluating count of LogLevel. {GetVerifyCountFailMessage(expectedLogLevel, timeCount)}";

            logger.VerifyLogging(expectedLogLevel, (v, t) => true, timeCount, failMessage);

            return logger;
        }

        private static string GetVerifyCountFailMessage(LogLevel expectedLogLevel, Times timeCount)
        {
            timeCount.Deconstruct(out int from, out int to);
            return $"Log Level:{expectedLogLevel}  Count Range: {from}-{to}";
        }

        private static string WrapMessageWithPadding(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("//*******************************************************************************************************************");
            sb.AppendLine("");
            sb.Append("    ");
            sb.Append(message);
            sb.Append(Environment.NewLine);
            sb.AppendLine("");
            sb.AppendLine("//*******************************************************************************************************************");
            sb.AppendLine("");
            return sb.ToString();
        }

        private static void VerifyLogging<T>(this Mock<ILogger<T>> logger,
            LogLevel expectedLogLevel,
            Func<object, Type, bool> messageVerificationFunc,
            Times times,
            string failMessage)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => messageVerificationFunc(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
               times, WrapMessageWithPadding(failMessage));
        }
    }
}