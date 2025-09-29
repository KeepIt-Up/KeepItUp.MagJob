using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Mock implementation of IKeycloakClient for integration tests.
/// </summary>
public class MockKeycloakClient : IKeycloakClient
{
    public Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<KeycloakUser?>(null);
    }

    public Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<KeycloakUser?>(null);
    }

    public Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<KeycloakUser>());
    }

    public Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("mock-user-id");
    }

    public Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult("mock-admin-token");
    }

    public Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<KeycloakUser>());
    }

    public Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<string>());
    }

    public Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<KeycloakRole>());
    }

    public Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("mock-role-id");
    }

    public Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<string?> GetUserProfilePictureUrlAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }
}