using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interfejs klienta do komunikacji z API Keycloak
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Pobiera użytkownika z Keycloak na podstawie identyfikatora
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje</returns>
    Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera użytkownika z Keycloak na podstawie adresu email
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje</returns>
    Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera listę użytkowników z Keycloak
    /// </summary>
    /// <param name="search">Opcjonalny parametr wyszukiwania</param>
    /// <param name="first">Indeks pierwszego elementu do pobrania</param>
    /// <param name="max">Maksymalna liczba elementów do pobrania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista użytkowników</returns>
    Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tworzy nowego użytkownika w Keycloak
    /// </summary>
    /// <param name="user">Dane użytkownika do utworzenia</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonego użytkownika</returns>
    Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktualizuje dane użytkownika w Keycloak
    /// </summary>
    /// <param name="user">Dane użytkownika do aktualizacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dezaktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktualizuje atrybuty użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="attributes">Atrybuty do zaktualizowania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera token dostępu do API Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Token dostępu</returns>
    Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera role użytkownika z Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista ról użytkownika</returns>
    Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Przypisuje rolę do użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Usuwa rolę użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera wszystkie dostępne role z Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista dostępnych ról</returns>
    Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tworzy nową rolę w Keycloak
    /// </summary>
    /// <param name="role">Dane roli do utworzenia</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonej roli</returns>
    Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktualizuje rolę w Keycloak
    /// </summary>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="role">Zaktualizowane dane roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Usuwa rolę z Keycloak
    /// </summary>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje status aktywności użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="enabled">Czy użytkownik ma być aktywny</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera wszystkich użytkowników z Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista użytkowników</returns>
    Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
} 
