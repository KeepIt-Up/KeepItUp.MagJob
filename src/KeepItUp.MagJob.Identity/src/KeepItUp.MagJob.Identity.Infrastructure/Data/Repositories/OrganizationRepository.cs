using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.Exceptions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;
using Npgsql;
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
        await _dbContext.Organizations.AddAsync(organization, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return organization;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        // For already tracked entities, EF Core will automatically detect changes
        // Force change detection to ensure all changes are tracked
        _dbContext.ChangeTracker.DetectChanges();
        await _dbContext.SaveChangesAsync(cancellationToken);
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
        var membersQuery = _dbContext.Set<Member>()
            .Where(m => m.OrganizationId == organizationId)
            .Include(m => m.Roles)
                .ThenInclude(r => r.Permissions);

        return await membersQuery.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }


    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetPermissionsWithPaginationAsync<TDestination>(
        Expression<Func<Permission, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Permissions.AsNoTracking();

        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetRolesByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId);

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
        var member = await _dbContext.Set<Member>()
            .AsNoTracking()
            .Include(m => m.Roles)
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == memberUserId, cancellationToken);

        if (member == null)
        {
            return PaginationResult<TDestination>.Create(
                new List<TDestination>(),
                0,
                parameters);
        }

        var roleIds = member.GetRoleIds().ToList();

        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId && roleIds.Contains(r.Id));

        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateRolePermissionsAsync(Guid roleId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Set<Role>()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
        {
            throw new EntityNotFoundException($"Role with ID {roleId} not found.");
        }

        role.ClearPermissions();

        var permissionsList = permissionNames.ToList();
        var existingPermissions = await _dbContext.Permissions
            .Where(p => permissionsList.Contains(p.Name))
            .ToListAsync(cancellationToken);

        var missingPermissionNames = permissionsList.Except(existingPermissions.Select(p => p.Name)).ToList();

        var newPermissions = missingPermissionNames.Select(name => new Permission(name)).ToList();
        if (newPermissions.Any())
        {
            await _dbContext.Permissions.AddRangeAsync(newPermissions, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var allPermissions = existingPermissions.Concat(newPermissions).ToList();

        foreach (var permission in allPermissions)
        {
            role.AddPermission(permission);
        }

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
        var organization = await _dbContext.Organizations
            .Include(o => o.Roles)
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);

        if (organization == null)
        {
            throw new EntityNotFoundException($"Organization with ID {organizationId} not found.");
        }

        organization.RemoveRole(roleId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a role to a member using direct SQL as a workaround for EF Core many-to-many issues.
    /// </summary>
    public async Task AddRoleToMemberAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.ExecuteSqlAsync($@"
            INSERT INTO identity.""MemberRoles"" (""MemberId"", ""RoleId"")
            VALUES ({memberId}, {roleId})
            ON CONFLICT (""MemberId"", ""RoleId"") DO NOTHING", cancellationToken);
    }

    /// <summary>
    /// Removes a role from a member using direct SQL as a workaround for EF Core many-to-many issues.
    /// </summary>
    public async Task RemoveRoleFromMemberAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.ExecuteSqlAsync($@"
            DELETE FROM identity.""MemberRoles""
            WHERE ""MemberId"" = {memberId} AND ""RoleId"" = {roleId}", cancellationToken);
    }
}
