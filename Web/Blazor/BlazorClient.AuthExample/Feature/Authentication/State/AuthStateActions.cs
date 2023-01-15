using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public class LoggedIn
{
    public string Name { get; init; }
    public Claim[] Claims { get; init; }
}

public class LoggedOut { }