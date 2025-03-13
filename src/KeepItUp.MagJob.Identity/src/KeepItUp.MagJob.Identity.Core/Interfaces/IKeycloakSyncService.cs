

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interfejs serwisu synchronizacji z Keycloak
/// </summary>
public interface IKeycloakSyncService
{
    /// <summary>
    /// Synchronizuje role użytkownika z Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task SyncUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Synchronizuje dane użytkownika z Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task SyncUserDataAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Synchronizuje wszystkich użytkowników z Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task SyncAllUsersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Importuje nowego użytkownika z Keycloak do modułu Identity
    /// </summary>
    /// <param name="keycloakUserId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonego użytkownika w module Identity</returns>
    Task<Guid> ImportUserFromKeycloakAsync(string keycloakUserId, CancellationToken cancellationToken = default);
} 
