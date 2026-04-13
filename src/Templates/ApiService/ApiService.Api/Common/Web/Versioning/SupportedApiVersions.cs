using Asp.Versioning;

namespace ApiService.Api.Common.Web.Versioning;

public static class SupportedApiVersions
{
    /// <summary>Default API version for this service.</summary>
    public static readonly ApiVersion V1 = new(1, 0);

    /// <summary>Header that carries the requested API version.</summary>
    public const string HeaderName = "X-Api-Version";

    /// <summary>Send this value with <see cref="HeaderName"/> to request <see cref="V1"/>.</summary>
    public const string V1RequestValue = "1.0";

    /// <summary>Single line for Refit <c>[Headers(...)]</c> applying <see cref="V1RequestValue"/>.</summary>
    public const string V1RefitHeaderLine = HeaderName + ": " + V1RequestValue;
}
