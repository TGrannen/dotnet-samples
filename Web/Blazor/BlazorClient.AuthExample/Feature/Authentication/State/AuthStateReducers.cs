using System.Security.Claims;

namespace BlazorClient.AuthExample.Feature.Authentication.State;

public class SessionStateReducers
{
    [ReducerMethod]
    public static AuthState Action1(AuthState state, LoggedIn action)
    {
        return state with
        {
            IsAuthenticated = true,
            Name = action.Name,
            Claims = action.Claims
        };
    }

    [ReducerMethod]
    public static AuthState Action2(AuthState state, LoggedOut _)
    {
        return state with
        {
            IsAuthenticated = false,
            Name = string.Empty,
            Claims = Array.Empty<Claim>()
        };
    }
}