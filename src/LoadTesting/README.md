# Load Testing

Load testing in .NET with [NBomber](https://nbomber.com/).

### Sample:

```c#
var scenario = ScenarioBuilder
    .CreateScenario("simple_http", step)
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        // Simulation.InjectPerSec(10, TimeSpan.FromSeconds(30))
        Simulation.KeepConstant(5, TimeSpan.FromSeconds(30))
    );

NBomberRunner
    .RegisterScenarios(scenario/*, fullExampleScenario*/)
    .Run();
```
