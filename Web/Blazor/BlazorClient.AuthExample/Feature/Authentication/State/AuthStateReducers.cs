using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public class SessionStateReducers
{
    [ReducerMethod]
    public static AuthState Action1(AuthState state, LoggedIn action)
    {
        var picture = action.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("picture"))?.Value ?? string.Empty;
        return state with
        {
            IsAuthenticated = true,
            Name = action.Name,
            Claims = action.Claims,
            HasImage = !string.IsNullOrEmpty(picture),
            ImageSrc = picture
        };
    }

    [ReducerMethod]
    public static AuthState Action2(AuthState state, LoggedOut _)
    {
        return state with
        {
            IsAuthenticated = false,
            Name = string.Empty,
            Claims = Array.Empty<Claim>(),
            HasImage = false,
            ImageSrc = string.Empty
        };
    }
}