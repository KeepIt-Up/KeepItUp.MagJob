using System.Linq.Expressions;
using System.Reflection;
using KeepItUp.MagJob.Identity.Core.Exceptions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;
namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// Implementation of the organization repository
/// </summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Initializes the repository instance
    /// </summary>
    public OrganizationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByIdWithRolesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByIdWithMembersAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Include(o => o.Members)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByIdWithMembersAndRolesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles)
            .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }


    /// <inheritdoc />
    public async Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Organization>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Organization>()
            .Include(o => o.Members.Where(m => m.UserId == userId))
                .ThenInclude(m => m.Roles)
            .Where(o => o.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasMemberAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Where(o => o.Id == organizationId)
            .AnyAsync(o => o.Members.Any(m => m.UserId == userId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Id == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        // Ensure member-role relationships are tracked
        foreach (var member in organization.Members)
        {
            // Make sure the Roles collection has references to actual Role entities
            var roleIds = member.RoleIds.ToList();
            member.Roles.Clear();

            foreach (var roleId in roleIds)
            {
                var role = organization.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role != null)
                {
                    member.Roles.Add(role);
                }
            }
        }

        await _dbContext.Organizations.AddAsync(organization, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return organization;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple update using EF Core change tracking
            // Domain methods should handle all business logic and state changes
            _dbContext.Organizations.Update(organization);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException($"Organization with ID {organization.Id} has been modified by another user.");
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _dbContext.Organizations.Remove(organization);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }


    /// <inheritdoc />
    public Task<List<Member>> GetMembersByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<Member>()
            .Include(m => m.Roles)
            .Where(m => m.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }


    public async Task<PaginationResult<TDestination>> GetOrganizationsByUserIdAsync<TDestination>(Guid userId, Expression<Func<Organization, TDestination>> selector, PaginationParameters<TDestination> parameters, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Organization>()
            .Include(o => o.Members.Where(m => m.UserId == userId))
                .ThenInclude(m => m.Roles)
            .Where(o => o.Members.Any(m => m.UserId == userId))
            .ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetMembersByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Member, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // Get the IQueryable for the members of the given organization
        var membersQuery = _dbContext.Set<Member>()
            .Where(m => m.OrganizationId == organizationId)
            .Include(m => m.Roles)
                .ThenInclude(r => r.Permissions);

        // Apply pagination using the PagedQueryableExtensions extension
        return await membersQuery.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }


    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetPermissionsWithPaginationAsync<TDestination>(
        Expression<Func<Permission, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // Get the IQueryable for the permissions
        var query = _dbContext.Permissions.AsNoTracking();

        // Return the paginated result
        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetRolesByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // Get the IQueryable for the roles of the organization
        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId);

        // Return the paginated result
        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetRolesByMemberIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Guid memberUserId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // First, get the member of the organization to get its role IDs
        var member = await _dbContext.Set<Member>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == memberUserId, cancellationToken);

        if (member == null)
        {
            // If the member does not exist, return an empty paginated collection
            return PaginationResult<TDestination>.Create(
                new List<TDestination>(),
                0,
                parameters);
        }

        // Get the role IDs of the member
        var roleIds = member.RoleIds;

        // Create a query for the roles of the member
        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId && roleIds.Contains(r.Id));

        // Return the paginated result
        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateRolePermissionsAsync(Guid roleId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default)
    {
        // Find the role in the database
        var role = await _dbContext.Set<Role>()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
        {
            throw new EntityNotFoundException($"Role with ID {roleId} not found.");
        }

        // Clear the current permissions
        role.ClearPermissions();

        // Get the permissions based on their names
        var permissionsList = permissionNames.ToList();
        var existingPermissions = await _dbContext.Permissions
            .Where(p => permissionsList.Contains(p.Name))
            .ToListAsync(cancellationToken);

        // Znajdź nazwy uprawnień, które nie istnieją w bazie danych
        var missingPermissionNames = permissionsList.Except(existingPermissions.Select(p => p.Name)).ToList();

        // Utwórz nowe uprawnienia dla brakujących nazw
        var newPermissions = missingPermissionNames.Select(name => new Permission(name)).ToList();
        if (newPermissions.Any())
        {
            await _dbContext.Permissions.AddRangeAsync(newPermissions, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Połącz istniejące i nowe uprawnienia
        var allPermissions = existingPermissions.Concat(newPermissions).ToList();

        // Add permissions
        foreach (var permission in allPermissions)
        {
            role.AddPermission(permission);
        }

        // Save the changes
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Delete the role from the organization.
    /// </summary>
    /// <param name="organizationId">Organization ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task DeleteRoleAsync(Guid organizationId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // Get the organization with roles and members
        var organization = await _dbContext.Organizations
            .Include(o => o.Roles)
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);

        if (organization == null)
        {
            throw new EntityNotFoundException($"Organization with ID {organizationId} not found.");
        }

        // Remove the role from the organization using the domain method
        organization.RemoveRole(roleId);

        // Save the changes
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
