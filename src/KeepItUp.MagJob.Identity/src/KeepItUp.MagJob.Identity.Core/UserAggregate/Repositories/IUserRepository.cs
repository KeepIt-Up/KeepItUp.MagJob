namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

/// <summary>
/// Repozytorium dla encji User
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Pobiera użytkownika po ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkownika po zewnętrznym ID (z systemu Keycloak)
    /// </summary>
    Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkownika po adresie email
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera aktywnych użytkowników
    /// </summary>
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkowników po ID organizacji
    /// </summary>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje użytkownika
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa użytkownika
    /// </summary>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza, czy użytkownik o podanym ID istnieje
    /// </summary>
    /// <param name="userId">ID użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>True, jeśli użytkownik istnieje; w przeciwnym razie false</returns>
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
