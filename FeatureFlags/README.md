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

## Open Feature Example

[OpenFeature](https://openfeature.dev/) is an open specification that provides a vendor-agnostic, community-driven API for feature flagging that works with your favorite feature
flag management tool or in-house solution. OpenFeature is designed to work with any feature flag management tool or in-house solution. This enables you to switch between platforms
or consolidate multiple platforms much more easily.
feature flags. They provide a .NET SDK ([Docs](https://openfeature.dev/docs/reference/technologies/server/dotnet)) that is used in the example project within this repo.

## LaunchDarkly Example & Library

[LaunchDarkly](https://launchdarkly.com) is a feature management platform that allows software development teams to
deliver to their customers. They provide a dotnet SDK to integrate with their
platform ([Docs](https://docs.launchdarkly.com/sdk/server-side/dotnet)). The included example project uses a second
library project to abstract away the Launch Darkly SDK just in case another feature management service is to be used in
the future.

## Split Example & Library

[Split](https://www.split.io/) is a unified feature flagging and experimentation platform enabling product and
engineering teams to reduce cycle times, mitigate release risk, and maximize business impact throughout the Feature
Delivery Lifecycle. They provide a dotnet SDK to integrate with their
platform ([Docs](https://help.split.io/hc/en-us/articles/360020240172--NET-SDK)). The included example project uses a
second library project to abstract away the Split SDK just in case another feature management service is to be used in
the future.

## Flagsmith Example & Library

[Flagsmith](https://flagsmith.com/) provides an all-in-one platform for developing, implementing, and managing your
feature flags. They provide a dotnet SDK to integrate with their
platform ([Docs](https://docs.flagsmith.com/clients/server-side)). The included example project uses a second library
project to abstract away the Flagsmith SDK just in case another feature management service is to be used in the future. 
