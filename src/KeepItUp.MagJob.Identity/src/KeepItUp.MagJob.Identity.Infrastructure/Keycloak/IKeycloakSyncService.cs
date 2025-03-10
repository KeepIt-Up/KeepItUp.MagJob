using System;
using System.Threading;
using System.Threading.Tasks;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Interfejs serwisu do synchronizacji danych między modułem Identity a Keycloak
/// </summary>
public interface IKeycloakSyncService
{
    /// <summary>
    /// Synchronizuje role i organizacje użytkownika z Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w module Identity</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task SyncUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Synchronizuje dane użytkownika z Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w module Identity</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task SyncUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    
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
