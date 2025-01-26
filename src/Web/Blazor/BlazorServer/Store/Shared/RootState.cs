namespace BlazorServer.Store.Shared;

public abstract class RootState
{
    protected RootState(bool isLoading, string? currentErrorMessage)
    {
        IsLoading = isLoading;
        CurrentErrorMessage = currentErrorMessage;
    }

    public bool IsLoading { get; }

    public string? CurrentErrorMessage { get; }

    public bool HasCurrentErrors => !string.IsNullOrWhiteSpace(CurrentErrorMessage);
}