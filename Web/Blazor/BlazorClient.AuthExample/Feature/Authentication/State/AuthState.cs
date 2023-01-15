using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public record AuthState
{
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public string Name { get; init; } = string.Empty;

    public ClaimsPrincipal? User { get; set; }
    public Claim[] Claims { get; init; } = Array.Empty<Claim>();
    public bool HasImage { get; init; }
    public string ImageSrc { get; init; } = string.Empty;
}

public class AuthStateFeature : Feature<AuthState>
{
    public override string GetName() => "Auth";

    protected override AuthState GetInitialState()
    {
        return new AuthState
        {
            Name = string.Empty,
            User = null,
            Claims = Array.Empty<Claim>(),
        };
    }
}