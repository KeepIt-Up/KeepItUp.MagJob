using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interfejs klienta do komunikacji z API Keycloak
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Pobiera użytkownika na podstawie identyfikatora
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Użytkownik Keycloak lub null, jeśli nie znaleziono</returns>
    Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkownika na podstawie adresu email
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Użytkownik Keycloak lub null, jeśli nie znaleziono</returns>
    Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkowników na podstawie kryteriów wyszukiwania
    /// </summary>
    /// <param name="search">Fraza wyszukiwania</param>
    /// <param name="first">Indeks pierwszego wyniku</param>
    /// <param name="max">Maksymalna liczba wyników</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista użytkowników Keycloak</returns>
    Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tworzy nowego użytkownika w Keycloak
    /// </summary>
    /// <param name="user">Dane użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonego użytkownika</returns>
    Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejącego użytkownika w Keycloak
    /// </summary>
    /// <param name="user">Dane użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje status aktywności użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="enabled">Nowy status aktywności</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dezaktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje atrybuty użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="attributes">Atrybuty do zaktualizowania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera token dostępu administratora Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Token dostępu</returns>
    Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wszystkich użytkowników z Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista wszystkich użytkowników</returns>
    Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera role użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista nazw ról przypisanych do użytkownika</returns>
    Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Przypisuje rolę do użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa rolę z użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="roleName">Nazwa roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wszystkie dostępne role z Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista ról</returns>
    Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tworzy nową rolę w Keycloak
    /// </summary>
    /// <param name="role">Dane roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonej roli</returns>
    Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejącą rolę w Keycloak
    /// </summary>
    /// <param name="roleName">Nazwa roli do aktualizacji</param>
    /// <param name="role">Nowe dane roli</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa rolę z Keycloak
    /// </summary>
    /// <param name="roleName">Nazwa roli do usunięcia</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);
}
