using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak.Models;
using Microsoft.AspNetCore.Authentication;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Klasa mapująca atrybuty użytkownika z Keycloak do tokenów JWT
/// </summary>
public class KeycloakAttributeMapper
{
    /// <summary>
    /// Mapuje atrybuty użytkownika z Keycloak do claimów
    /// </summary>
    /// <param name="user">Użytkownik z Keycloak</param>
    /// <returns>Lista claimów</returns>
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
            // Dodaj organizacje jako claim
            if (user.Attributes.TryGetValue("organizations", out var organizationsValue) && organizationsValue.Count > 0)
            {
                claims.Add(new Claim("organizations", organizationsValue[0]));
            }
            
            // Dodaj uprawnienia jako claim
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
    /// Rozszerza token JWT o dodatkowe informacje z Keycloak
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
            
            // Pobierz organizacje z tokenu
            var organizationsClaim = claimsIdentity.FindFirst("organizations")?.Value;
            if (!string.IsNullOrEmpty(organizationsClaim))
            {
                try
                {
                    var organizations = JsonSerializer.Deserialize<List<KeycloakOrganization>>(organizationsClaim);
                    if (organizations != null)
                    {
                        // Dodaj claim dla każdej organizacji
                        foreach (var org in organizations)
                        {
                            claimsIdentity.AddClaim(new Claim("organization", org.Id));
                            
                            // Dodaj claim dla każdej roli w organizacji
                            foreach (var role in org.Roles)
                            {
                                claimsIdentity.AddClaim(new Claim($"role_{org.Id}", role));
                            }
                            
                            // Dodaj claim dla uprawnień w kontekście organizacji
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
                                    // Logowanie błędu deserializacji
                                    Console.WriteLine($"Błąd deserializacji uprawnień: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Logowanie błędu deserializacji
                    Console.WriteLine($"Błąd deserializacji organizacji: {ex.Message}");
                }
            }
            
            return Task.FromResult(principal);
        }
    }
} 
