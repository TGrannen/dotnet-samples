# Caching

Caching is the process of storing the data that’s frequently used so that data can be served faster for any future requests. ASP.NET provides both implementation and abstractions
for local memory and distributed caches. This project showcases both caching styles with the usage of FusionCache. See
the [.NET Caching Docs](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview) for more information.

## [FusionCache](https://github.com/ZiggyCreatures/FusionCache?tab=readme-ov-file)

FusionCache is an easy to use, fast and robust hybrid cache with advanced resiliency features. [.NET Live Video](https://www.youtube.com/watch?v=hCswI2goi7s)

[Step By Step](https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/StepByStep.md) Fusion Cache setup

## Running

Run the solution from the **Caching.AppHost** project. The AppHost starts Redis (with Redis Insight), the Web API, and the Aspire Dashboard.

## Links

When running via the AppHost:

- **Aspire Dashboard** – [http://localhost:18888/](http://localhost:18888/) (or the port shown in the console)
- **Redis Insight** – available from the dashboard when Redis is configured with `WithRedisInsight()`
