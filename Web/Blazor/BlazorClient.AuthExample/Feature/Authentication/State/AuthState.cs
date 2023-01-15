using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public record AuthState
{
    public bool IsAuthenticated { get; init; }
    public string Name { get; init; }
    public Claim[] Claims { get; init; }
    public bool HasImage { get; init; }
    public string ImageSrc { get; init; }
}

public class AuthStateFeature : Feature<AuthState>
{
    public override string GetName() => "Auth";

    protected override AuthState GetInitialState()
    {
        return new AuthState
        {
            IsAuthenticated = false,
            Name = string.Empty,
            Claims = Array.Empty<Claim>(),
        };
    }
}