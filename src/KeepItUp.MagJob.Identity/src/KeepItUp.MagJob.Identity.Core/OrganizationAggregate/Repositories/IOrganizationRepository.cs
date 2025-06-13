using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;

/// <summary>
/// Repository for the Organization entity.
/// </summary>
public interface IOrganizationRepository
{
    /// <summary>
    /// Gets an organization by its ID.
    /// </summary>
    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an organization by its ID with roles.
    /// </summary>
    Task<Organization?> GetByIdWithRolesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an organization by its ID with members.
    /// </summary>
    Task<Organization?> GetByIdWithMembersAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an organization by its ID with members and roles.
    /// </summary>
    Task<Organization?> GetByIdWithMembersAndRolesAsync(Guid organizationId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Gets an organization by its name.
    /// </summary>
    Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets organizations for a given user.
    /// </summary>
    Task<List<Organization>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is a member of an organization.
    /// </summary>
    Task<bool> HasMemberAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an organization with the given ID exists.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True, if the organization exists; otherwise false.</returns>
    Task<bool> ExistsAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an organization with the given name exists.
    /// </summary>
    /// <param name="name">Organization name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True, if the organization exists; otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an organization.
    /// </summary>
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an organization.
    /// </summary>
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an organization.
    /// </summary>
    Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets members of an organization by its ID.
    /// </summary>
    Task<List<Member>> GetMembersByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Updates a role's permissions.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    /// <param name="permissionNames">List of permission names to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task</returns>
    Task UpdateRolePermissionsAsync(Guid roleId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role from an organization.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task</returns>
    Task DeleteRoleAsync(Guid organizationId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of organizations for a given user.
    /// </summary>
    Task<PaginationResult<TDestination>> GetOrganizationsByUserIdAsync<TDestination>(Guid userId, Expression<Func<Organization, TDestination>> selector, PaginationParameters<TDestination> parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of members of an organization.
    /// </summary>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="selector">Selector mapping from Member to TDestination.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pagination result.</returns>
    Task<PaginationResult<TDestination>> GetMembersByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Member, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Gets a paginated list of permissions.
    /// </summary>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="selector">Selector mapping from Permission to TDestination.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated result of permissions.</returns>
    Task<PaginationResult<TDestination>> GetPermissionsWithPaginationAsync<TDestination>(
        Expression<Func<Permission, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of roles for an organization.
    /// </summary>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="selector">Selector mapping from Role to TDestination.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pagination result.</returns>
    Task<PaginationResult<TDestination>> GetRolesByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of roles for a member of an organization.
    /// </summary>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="memberUserId">User ID of the member.</param>
    /// <param name="selector">Selector mapping from Role to TDestination.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pagination result.</returns>
    Task<PaginationResult<TDestination>> GetRolesByMemberIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Guid memberUserId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default);
}
