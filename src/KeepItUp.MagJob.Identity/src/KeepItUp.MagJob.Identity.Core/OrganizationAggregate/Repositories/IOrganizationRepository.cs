namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;

/// <summary>
/// Repozytorium dla encji Organization
/// </summary>
public interface IOrganizationRepository
{
    /// <summary>
    /// Pobiera organizację po ID
    /// </summary>
    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizację po ID wraz z rolami
    /// </summary>
    Task<Organization?> GetByIdWithRolesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizację po ID wraz z członkami
    /// </summary>
    Task<Organization?> GetByIdWithMembersAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizację po ID wraz z członkami i rolami
    /// </summary>
    Task<Organization?> GetByIdWithMembersAndRolesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizację po ID wraz z zaproszeniami
    /// </summary>
    Task<Organization?> GetByIdWithInvitationsAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizację po nazwie
    /// </summary>
    Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera organizacje dla danego użytkownika
    /// </summary>
    Task<List<Organization>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza czy użytkownik jest członkiem organizacji
    /// </summary>
    Task<bool> HasMemberAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza czy organizacja o podanym identyfikatorze istnieje
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>True, jeśli organizacja istnieje; w przeciwnym razie false</returns>
    Task<bool> ExistsAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza czy organizacja o podanej nazwie istnieje
    /// </summary>
    /// <param name="name">Nazwa organizacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>True, jeśli organizacja istnieje; w przeciwnym razie false</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje organizację
    /// </summary>
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje organizację
    /// </summary>
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa organizację
    /// </summary>
    Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default);
}
