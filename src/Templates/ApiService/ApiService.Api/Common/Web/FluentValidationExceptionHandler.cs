using Microsoft.AspNetCore.Diagnostics;

namespace ApiService.Api.Common.Web;

public sealed class FluentValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException vx)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errors = vx.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        await httpContext.Response.WriteAsJsonAsync(
            new HttpValidationProblemDetails(errors),
            cancellationToken: cancellationToken);

        return true;
    }
}
