namespace TUnitTesting.WireMockTests.Extensions;

public static class StringExtensions
{
    public static string EnsureTrailingSlash(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.EndsWith('/') ? value : value + "/";
    }
}
