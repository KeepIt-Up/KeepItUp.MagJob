using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;

namespace KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;

/// <summary>
/// Repository for the Invitation entity.
/// </summary>
public interface IInvitationRepository
{
    /// <summary>
    /// Gets an invitation by its ID.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Invitation if found; otherwise null.</returns>
    Task<Invitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an invitation by its token.
    /// </summary>
    /// <param name="token">Invitation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Invitation if found; otherwise null.</returns>
    Task<Invitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invitations by organization ID.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invitations for the organization.</returns>
    Task<List<Invitation>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invitations by email address.
    /// </summary>
    /// <param name="email">Email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invitations for the email address.</returns>
    Task<List<Invitation>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending invitations for an organization.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of pending invitations.</returns>
    Task<List<Invitation>> GetPendingByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a pending invitation exists for the email and organization.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="email">Email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if pending invitation exists; otherwise false.</returns>
    Task<bool> HasPendingInvitationAsync(Guid organizationId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an invitation with the given ID exists.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the invitation exists; otherwise false.</returns>
    Task<bool> ExistsAsync(Guid invitationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new invitation.
    /// </summary>
    /// <param name="invitation">Invitation to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Added invitation.</returns>
    Task<Invitation> AddAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an invitation.
    /// </summary>
    /// <param name="invitation">Invitation to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an invitation.
    /// </summary>
    /// <param name="invitation">Invitation to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task DeleteAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of invitations for an organization.
    /// </summary>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="selector">Selector mapping from Invitation to TDestination.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="filter">Optional filter for invitations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pagination result.</returns>
    Task<PaginationResult<TDestination>> GetByOrganizationIdWithPaginationAsync<TDestination>(
        Expression<Func<Invitation, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        Expression<Func<Invitation, bool>>? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired invitations that need to be marked as expired.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of expired invitations.</returns>
    Task<List<Invitation>> GetExpiredInvitationsAsync(CancellationToken cancellationToken = default);
}