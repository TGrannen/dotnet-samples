namespace Refit.ConsoleApp;

public interface IYesOrNoApi
{
    [Get("/api")]
    Task<YesOrNoResponse> GetResponse();

    [Get("/api")]
    Task<YesOrNoResponse> ForceResponse(string force);
}