# Polly

This project is designed to illustrate the capabilities of the [Polly](https://github.com/App-vNext/Polly) NuGet package for make resilience and transient-fault-handling code in .Net.

This project has sets up a `Flaky` server that will not always respond with a 200 on some of its endpoints and a simulated client `Web` project that performs calls to the Flaky server upon its own endpoint requests. This uses the `HttpClientFactory` injection to allow for wrapping the Polly Polices around all http calls from a given factory.

This project also includes a use of Polly without the HttpClientFactory in the `GithubService` class. This illustrates that Polly can be used with any code and not just http requests (even though that's still somewhat the scenario in the example).
