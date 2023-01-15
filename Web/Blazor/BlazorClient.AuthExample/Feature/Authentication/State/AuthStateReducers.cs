using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public class SessionStateReducers
{
    [ReducerMethod]
    public static AuthState Action1(AuthState state, LoggedIn action)
    {
        var user = action.User;
        var claims = user.Claims.ToArray();
        var picture = claims.FirstOrDefault(x => x.Type.ToLower().Equals("picture"))?.Value ?? string.Empty;
        return state with
        {
            User = user,
            Name = user.Identity?.Name ?? string.Empty,
            Claims = claims,
            HasImage = !string.IsNullOrEmpty(picture),
            ImageSrc = picture
        };
    }

    [ReducerMethod]
    public static AuthState Action2(AuthState state, LoggedOut _)
    {
        return state with
        {
            User = null,
            Name = string.Empty,
            Claims = Array.Empty<Claim>(),
            HasImage = false,
            ImageSrc = string.Empty
        };
    }
}