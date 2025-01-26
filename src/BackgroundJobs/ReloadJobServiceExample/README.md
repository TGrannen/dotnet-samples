# Reload Job Example

This project contains a custom implementation of what I've named a "Reload Job". This means that a Job class within the
Assembly will be executed any time its `IReloadJobService.Reload()` method is called and will continue to execute until
it has returned successfully.
