using System.Security.Claims;
using System.Text.Json;
using KeepItUp.MagJob.Identity.Core.Keycloak;
using Microsoft.AspNetCore.Authentication;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Class mapping user attributes from Keycloak to JWT tokens
/// </summary>
public class KeycloakAttributeMapper
{
    /// <summary>
    /// Maps user attributes from Keycloak to claims
    /// </summary>
    /// <param name="user">User from Keycloak</param>
    /// <returns>List of claims</returns>
    public static List<Claim> MapUserAttributesToClaims(KeycloakUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (!string.IsNullOrEmpty(user.FirstName))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
        }

        if (!string.IsNullOrEmpty(user.LastName))
        {
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
        }

        if (user.Attributes != null)
        {
            // Add organizations as a claim
            if (user.Attributes.TryGetValue("organizations", out var organizationsValue) && organizationsValue.Count > 0)
            {
                claims.Add(new Claim("organizations", organizationsValue[0]));
            }

            // Add permissions as a claim
            if (user.Attributes.TryGetValue("permissions", out var permissionsValue) && permissionsValue.Count > 0)
            {
                foreach (var permission in permissionsValue)
                {
                    claims.Add(new Claim("permission", permission));
                }
            }
        }

        return claims;
    }

    /// <summary>
    /// Extends the JWT token with additional information from Keycloak
    /// </summary>
    public class KeycloakClaimsTransformation : IClaimsTransformation
    {
        /// <inheritdoc />
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
            {
                return Task.FromResult(principal);
            }

            // Get organizations from the token
            var organizationsClaim = claimsIdentity.FindFirst("organizations")?.Value;
            if (!string.IsNullOrEmpty(organizationsClaim))
            {
                try
                {
                    var organizations = JsonSerializer.Deserialize<List<KeycloakOrganization>>(organizationsClaim);
                    if (organizations != null)
                    {
                        // Add claim for each organization
                        foreach (var org in organizations)
                        {
                            claimsIdentity.AddClaim(new Claim("organization", org.Id));

                            // Add claim for each role in the organization
                            foreach (var role in org.Roles)
                            {
                                claimsIdentity.AddClaim(new Claim($"role_{org.Id}", role));
                            }

                            // Add claim for permissions in the organization context
                            var permissionsClaim = claimsIdentity.FindFirst("permissions")?.Value;
                            if (!string.IsNullOrEmpty(permissionsClaim))
                            {
                                try
                                {
                                    var permissionsMap = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(permissionsClaim);
                                    if (permissionsMap != null && permissionsMap.TryGetValue(org.Id, out var orgPermissions))
                                    {
                                        foreach (var permission in orgPermissions)
                                        {
                                            claimsIdentity.AddClaim(new Claim($"permission_{org.Id}", permission));
                                        }
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    // Logging the deserialization error
                                    Console.WriteLine($"Błąd deserializacji uprawnień: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Logging the deserialization error
                    Console.WriteLine($"Błąd deserializacji organizacji: {ex.Message}");
                }
            }

            return Task.FromResult(principal);
        }
    }
}
