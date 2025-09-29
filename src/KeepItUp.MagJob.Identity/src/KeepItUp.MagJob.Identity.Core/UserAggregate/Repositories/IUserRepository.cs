namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

/// <summary>
/// Repository for the User entity
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by external ID (from Keycloak)
    /// </summary>
    Task<User?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active users
    /// </summary>
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by list of IDs
    /// </summary>
    /// <param name="userIds">List of user IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    Task<List<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by organization ID
    /// </summary>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given ID exists
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True, if the user exists; otherwise false</returns>
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
