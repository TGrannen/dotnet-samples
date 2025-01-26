# Configuration

This project shows various aspects of setting up the built
in [Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration) system in .Net.

This is an ASP.NET Web API project that exposes a single endpoint for viewing the currently active example settings
configured in the solution. Configuration sources and concepts that are shown in the project are listed below.

* Settings files, such as appsettings.json
* Overriding Configuration, such as appsettings.Development.json
* Environment variables
* User Secrets
* Command-line arguments
* Abstracted Settings from the Configuration Library
* Custom Configuration Provider with mutable configuration at runtime
* Validated configuration at startup
  with [FluentValidation](https://github.com/FluentValidation/FluentValidation) - [Tutorial Video](https://www.youtube.com/watch?v=jblRYDMTtvg)
* AWS Parameter Store - [Tutorial Video](https://www.youtube.com/watch?v=J0EVd5HbtUY)