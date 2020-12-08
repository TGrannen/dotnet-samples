using System.Threading.Tasks;

// WARN: This Refit using statement is required to properly generate the API implementation
using Refit;

namespace Refit.ConsoleApp
{
    public interface IYesOrNoApi
    {
        [Get("/api")]
        Task<YesOrNoResponse> GetResponse();

        [Get("/api")]
        Task<YesOrNoResponse> ForceResponse(string force);
    }
}