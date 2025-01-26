using NBomber.Contracts;
using NBomber.CSharp;

var httpFactory = ClientFactory.Create(
    name: "http_factory",
    clientCount: 1,
    initClient: (number, context) => Task.FromResult(new HttpClient())
);

var step = Step.Create("get_weather_forecasts", httpFactory, async context =>
{
    var response = await context.Client.GetAsync("http://localhost:5000/WeatherForecast", context.CancellationToken);

    return response.IsSuccessStatusCode
        ? Response.Ok(statusCode: (int)response.StatusCode)
        : Response.Fail(statusCode: (int)response.StatusCode);
});

var scenario = ScenarioBuilder
    .CreateScenario("simple_http", step)
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        // Simulation.InjectPerSec(10, TimeSpan.FromSeconds(30))
        Simulation.KeepConstant(5, TimeSpan.FromSeconds(30))
    );

var fullExampleScenario = GenerateFullExampleScenario(step);

NBomberRunner
    .RegisterScenarios(scenario /*, fullExampleScenario*/)
    .Run();

static Scenario GenerateFullExampleScenario(IStep step)
{
    var scenario = ScenarioBuilder
        .CreateScenario("simple_http", step)
        .WithWarmUpDuration(TimeSpan.FromSeconds(5))
        .WithInit(async context => { await Task.Delay(2000); })
        .WithClean(async context => { await Task.Delay(2000); })
        .WithLoadSimulations(
            // will create a 5 copies (threads) of the current scenario 
            // and run them concurrently for 10 sec    
            // here every single copy will iterate while the specified duration
            Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(10)),

            // will inject 10 new copies (threads) per 1 sec
            // the copies will be injected at regular intervals
            // here every single copy will run only once
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(30)),

            // will inject a random number of scenario copies (threads) per 1 sec
            // the copies will be injected at regular intervals
            // here every single copy will run only once
            Simulation.InjectPerSecRandom(minRate: 50, maxRate: 500, during: TimeSpan.FromSeconds(20)),

            // It's to model a closed system.
            // Injects a given number of scenario copies (threads) 
            // with a linear ramp over a given duration.
            // Every single scenario copy will iterate while the specified duration.
            // Use it for ramp up and rump down.
            Simulation.RampConstant(copies: 10, during: TimeSpan.FromSeconds(20)),

            // It's to model a closed system.    
            // A fixed number of scenario copies (threads) executes as many iterations
            // as possible for a specified amount of time.
            // Every single scenario copy will iterate while the specified duration.
            // Use it when you need to run a specific amount of scenario copies (threads)
            // for a certain amount of time.        
            Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(10)),

            // It's to model an open system.
            // Injects a given number of scenario copies (threads) per 1 sec 
            // from the current rate to target rate during a given duration.     
            // Every single scenario copy will run only once.
            Simulation.RampPerSec(rate: 10, during: TimeSpan.FromSeconds(20)),

            // It's to model an open system.
            // Injects a given number of scenario copies (threads) per 1 sec
            // during a given duration. 
            // Every single scenario copy will run only once.
            // Use it when you want to maintain a constant rate of requests 
            // without being affected by the performance of the system under test.
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(20)),

            // It's to model an open system.
            // Injects a random number of scenario copies (threads) per 1 sec 
            // defined in scenarios per second during a given duration.
            // Every single scenario copy will run only once.
            // Use it when you want to maintain a random rate of requests
            // without being affected by the performance of the system under test.
            Simulation.InjectPerSecRandom(minRate: 10, maxRate: 50, during: TimeSpan.FromSeconds(20))
        );
    return scenario;
}