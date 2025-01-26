using Moq;
using SerilogExample.Web.Controllers;

namespace SerilogExample.Web.Tests;

public class WeatherForecastControllerTests
{
    private readonly Mock<ILogger<WeatherForecastController>> _loggerMock = new();
    private readonly WeatherForecastController _sut;

    public WeatherForecastControllerTests()
    {
        _sut = new WeatherForecastController(_loggerMock.Object);
    }

    [Fact]
    public void Get_ShouldLogMessage()
    {
        _sut.Get();

        _loggerMock.VerifyLog(x => x.LogInformation("Getting the weather forecasts!"));
    }

    [Fact]
    public void TestLog_ShouldLogMessage()
    {
        const int value = 456165;

        _sut.TestLog(value);

        _loggerMock.VerifyLog(x => x.LogInformation("This Value is {Count} and it's pretty cool", value));
    }
}