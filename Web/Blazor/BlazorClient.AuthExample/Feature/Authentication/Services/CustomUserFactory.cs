using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace BlazorClient.AuthExample.Feature.Authentication.Services;

public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public CustomUserFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
    {
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        var claimsIdentity = (ClaimsIdentity?)user.Identity;

        if (account == null || claimsIdentity == null)
        {
            return user;
        }

        foreach (var prop in account.AdditionalProperties)
        {
            var key = prop.Key;
            var value = prop.Value;
            if (value is JsonElement { ValueKind: JsonValueKind.Array } element)
            {
                // Remove the Roles claim with an array value and create a separate one for each role.
                claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(prop.Key));
                var claims = element.EnumerateArray().Select(x => new Claim(prop.Key, x.ToString()));
                claimsIdentity.AddClaims(claims);
            }
        }

        return user;
    }
}