using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public class LoggedIn
{
    public ClaimsPrincipal User { get; set; }
}

public class LoggedOut { }