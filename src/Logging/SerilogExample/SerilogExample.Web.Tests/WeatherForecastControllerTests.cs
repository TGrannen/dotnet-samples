using SerilogExample.Web.Controllers;

namespace SerilogExample.Web.Tests;

public class WeatherForecastControllerTests
{
    private readonly LoggingFixture _loggingFixture = new();
    private readonly WeatherForecastController _sut;

    public WeatherForecastControllerTests()
    {
        _sut = new WeatherForecastController(_loggingFixture.GetLogger<WeatherForecastController>());
    }

    [Fact]
    public void Get_ShouldLogMessage()
    {
        _sut.Get();

        _loggingFixture.Instance.Should().HaveMessage("Getting the weather forecasts!");
    }

    [Fact]
    public void TestLog_ShouldLogMessage()
    {
        const int value = 456165;

        _sut.TestLog(value);

        _loggingFixture.Instance.Should()
            .HaveMessage("This Value is {Count} and it's pretty cool")
            .Appearing().Once()
            .WithProperty("Count")
            .WithValue(value);
    }
}