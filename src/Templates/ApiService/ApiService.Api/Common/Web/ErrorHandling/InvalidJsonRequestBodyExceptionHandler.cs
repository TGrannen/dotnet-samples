using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace ApiService.Api.Common.Web.ErrorHandling;

public sealed class InvalidJsonRequestBodyExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException)
        {
            return false;
        }

        if (FindJsonException(exception) is not { } jsonException)
        {
            return false;
        }

        var fieldKey = JsonPathToFieldKey(jsonException.Path);
        var message = GetLeafErrorMessage(jsonException);

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [fieldKey] = [message],
        };

        var problem = new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid JSON request body",
            Detail = BuildDetail(jsonException, message),
        };

        if (!string.IsNullOrEmpty(jsonException.Path))
        {
            problem.Extensions["jsonPath"] = jsonException.Path;
        }

        if (jsonException.LineNumber is { } line)
        {
            problem.Extensions["line"] = line;
        }

        if (jsonException.BytePositionInLine is { } col)
        {
            problem.Extensions["bytePositionInLine"] = col;
        }

        await problemDetailsService.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception,
            });

        return true;
    }

    private static JsonException? FindJsonException(Exception exception)
    {
        for (var ex = exception; ex is not null; ex = ex.InnerException)
        {
            if (ex is JsonException jsonEx)
            {
                return jsonEx;
            }
        }

        return null;
    }

    private static string JsonPathToFieldKey(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || path is "$")
        {
            return "requestBody";
        }

        return path.StartsWith("$.", StringComparison.Ordinal) ? path[2..] : path.TrimStart('$', '.');
    }

    private static string GetLeafErrorMessage(JsonException jsonException)
    {
        Exception? deepest = jsonException;
        for (var inner = jsonException.InnerException; inner is not null; inner = inner.InnerException)
        {
            deepest = inner;
        }

        if (deepest is not null && deepest is not JsonException && !string.IsNullOrWhiteSpace(deepest.Message))
        {
            return deepest.Message;
        }

        var msg = jsonException.Message;
        var pathIdx = msg.IndexOf(" Path:", StringComparison.Ordinal);
        if (pathIdx > 0)
        {
            msg = msg[..pathIdx].TrimEnd();
        }

        return msg;
    }

    private static string BuildDetail(JsonException jsonException, string message)
    {
        var pathSegment = string.IsNullOrEmpty(jsonException.Path)
            ? "the request body"
            : $"JSON path '{jsonException.Path}'";

        var location = (jsonException.LineNumber, jsonException.BytePositionInLine) switch
        {
            ({ } line, { } col) => $" (line {line}, byte position {col})",
            ({ } line, _) => $" (line {line})",
            _ => string.Empty,
        };

        return $"Invalid value at {pathSegment}{location}: {message}";
    }
}
