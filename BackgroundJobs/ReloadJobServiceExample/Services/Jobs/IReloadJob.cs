namespace ReloadJobServiceExample.Services.Jobs;

/// <summary>
/// Will be executed when application indicates that it should be reloaded and will continue to retry until it is successful
/// </summary>
public interface IReloadJob
{
    Task<bool> Execute();
}