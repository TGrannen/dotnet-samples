# Feature Flags

This project shows various aspects of
incorporating [Feature Flags](https://docs.microsoft.com/en-us/dotnet/architecture/cloud-native/feature-flags) into a
project.

This is an ASP.NET Web API project that exposes a couple simple endpoints for testing different ways to setup feature
flags.

### Sample Code

```c#
if (await _featureManager.IsEnabledAsync("ShowWeather"))
{
    // Whatever you do when the feature is enabled
}
```
appsettings.json
```json
{
  "FeatureManagement": {
    "ShowWeather": true
  }
}
```

### Topics Covered

* General usage of the
  provided [Microsoft.FeatureManagement.AspNetCore](https://github.com/microsoft/FeatureManagement-Dotnet) package
* Configuration based setup of Feature Flags
* Time & Percentage based Feature Filters
* Custom Feature Filters
* Custom abstraction over Microsoft provided library
* [Azure App Configuration](https://azure.microsoft.com/en-us/services/app-configuration/#overview) Feature Management
  integration

