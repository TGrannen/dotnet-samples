# Docker Manipulation from .NET

Docker Manipulation in .NET with [FluentDocker](https://github.com/mariotoffia/FluentDocker).

### Sample:

```c#
using (
    var container =
      new Builder().UseContainer()
        .UseImage("kiasaki/alpine-postgres")
        .ExposePort(5432)
        .WithEnvironment("POSTGRES_PASSWORD=mysecretpassword")
        .WaitForPort("5432/tcp", 30000 /*30s*/)
        .Build()
        .Start())
  {
    var config = container.GetConfiguration(true);
    Assert.AreEqual(ServiceRunningState.Running, config.State.ToServiceState());
  }
```
