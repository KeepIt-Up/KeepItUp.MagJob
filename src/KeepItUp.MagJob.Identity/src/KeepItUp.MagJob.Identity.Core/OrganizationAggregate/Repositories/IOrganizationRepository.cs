using System.Linq.Expressions;
using KeepItUp.MagJob.SharedKernel.Pagination;

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

    /// <summary>
    /// Pobiera członków organizacji po ID organizacji.
    /// </summary>
    Task<List<Member>> GetMembersByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera zaproszenia do organizacji po ID organizacji.
    /// </summary>
    Task<List<Invitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera zaproszenie po ID.
    /// </summary>
    Task<Invitation?> GetInvitationByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje nowe zaproszenie.
    /// </summary>
    Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje zaproszenie.
    /// </summary>
    Task UpdateInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa zaproszenie.
    /// </summary>
    Task DeleteInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę organizacji dla danego użytkownika.
    /// </summary>
    Task<PaginationResult<TDestination>> GetOrganizationsByUserIdAsync<TDestination>(Guid userId, Expression<Func<Organization, TDestination>> selector, PaginationParameters<TDestination> parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę członków organizacji.
    /// </summary>
    /// <typeparam name="TDestination">Typ docelowy</typeparam>
    /// <param name="organizationId">Identyfikator organizacji</param>
    /// <param name="selector">Selektor mapujący z Member na TDestination</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik paginacji</returns>
    Task<PaginationResult<TDestination>> GetMembersByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Member, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę zaproszeń do organizacji.
    /// </summary>
    /// <typeparam name="TDestination">Typ docelowy</typeparam>
    /// <param name="organizationId">Identyfikator organizacji</param>
    /// <param name="selector">Selektor mapujący z Invitation na TDestination</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <param name="filter">Opcjonalny filtr dla zaproszeń (np. tylko oczekujące)</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik paginacji</returns>
    Task<PaginationResult<TDestination>> GetInvitationsByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Invitation, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        Expression<Func<Invitation, bool>>? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera listę uprawnień z paginacją.
    /// </summary>
    /// <typeparam name="TDestination">Typ docelowy, na który mapowane są uprawnienia.</typeparam>
    /// <param name="selector">Selektor określający mapowanie z Permission na typ docelowy.</param>
    /// <param name="parameters">Parametry paginacji.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Spaginowany wynik uprawnień.</returns>
    Task<PaginationResult<TDestination>> GetPermissionsWithPaginationAsync<TDestination>(
        Expression<Func<Permission, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę ról dla organizacji.
    /// </summary>
    /// <typeparam name="TDestination">Typ docelowy</typeparam>
    /// <param name="organizationId">Identyfikator organizacji</param>
    /// <param name="selector">Selektor mapujący z Role na TDestination</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik paginacji</returns>
    Task<PaginationResult<TDestination>> GetRolesByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę ról dla członka organizacji.
    /// </summary>
    /// <typeparam name="TDestination">Typ docelowy</typeparam>
    /// <param name="organizationId">Identyfikator organizacji</param>
    /// <param name="memberUserId">Identyfikator użytkownika członka</param>
    /// <param name="selector">Selektor mapujący z Role na TDestination</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik paginacji</returns>
    Task<PaginationResult<TDestination>> GetRolesByMemberIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Guid memberUserId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);
}
