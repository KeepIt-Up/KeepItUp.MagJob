using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.SharedKernel.Pagination;
namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// Implementacja repozytorium organizacji
/// </summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Inicjalizuje instancję repozytorium
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
            .AsNoTracking()
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
            .Include(o => o.Invitations)
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
            .Include(o => o.Invitations)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByIdWithInvitationsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Include(o => o.Invitations)
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
        // First check and detach any existing tracked entities with the same ID
        var existingOrgEntry = _dbContext.ChangeTracker.Entries<Organization>()
            .FirstOrDefault(e => e.Entity.Id == organization.Id);

        if (existingOrgEntry != null)
        {
            existingOrgEntry.State = EntityState.Detached;
        }

        // Check and detach any members that might be tracked
        foreach (var member in organization.Members)
        {
            var existingMemberEntry = _dbContext.ChangeTracker.Entries<Member>()
                .FirstOrDefault(e => e.Entity.Id == member.Id);

            if (existingMemberEntry != null)
            {
                existingMemberEntry.State = EntityState.Detached;
            }
        }

        // Check and detach any roles that might be tracked
        foreach (var role in organization.Roles)
        {
            var existingRoleEntry = _dbContext.ChangeTracker.Entries<Role>()
                .FirstOrDefault(e => e.Entity.Id == role.Id);

            if (existingRoleEntry != null)
            {
                existingRoleEntry.State = EntityState.Detached;
            }
        }

        // Handle member-role relationships within a transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
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

            _dbContext.Organizations.Update(organization);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
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

    /// <inheritdoc />
    public Task<List<Invitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Invitation?> GetInvitationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task UpdateInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DeleteInvitationAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
        // Pobierz najpierw IQueryable dla członków danej organizacji
        var membersQuery = _dbContext.Set<Member>()
            .Where(m => m.OrganizationId == organizationId)
            .Include(m => m.Roles)
                .ThenInclude(r => r.Permissions);

        // Zastosuj paginację używając rozszerzenia PagedQueryableExtensions
        return await membersQuery.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetInvitationsByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Invitation, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        Expression<Func<Invitation, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        // Pobierz IQueryable dla zaproszeń danej organizacji
        var invitationsQuery = _dbContext.Set<Invitation>()
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId);

        // Jeśli podano filtr, zastosuj go
        if (filter != null)
        {
            invitationsQuery = invitationsQuery.Where(filter);
        }

        // Zastosuj paginację używając rozszerzenia PagedQueryableExtensions
        return await invitationsQuery.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetPermissionsWithPaginationAsync<TDestination>(
        Expression<Func<Permission, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // Pobieramy IQueryable dla uprawnień
        var query = _dbContext.Permissions.AsNoTracking();

        // Zwracamy spaginowany wynik
        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetRolesByOrganizationIdWithPaginationAsync<TDestination>(
        Guid organizationId,
        Expression<Func<Role, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {
        // Pobieramy IQueryable dla ról organizacji
        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId);

        // Zwracamy spaginowany wynik
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
        // Najpierw pobieramy członka organizacji aby uzyskać jego identyfikatory ról
        var member = await _dbContext.Set<Member>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == memberUserId, cancellationToken);

        if (member == null)
        {
            // Jeśli członek nie istnieje, zwracamy pustą stronicowaną kolekcję
            return PaginationResult<TDestination>.Create(
                new List<TDestination>(),
                0,
                parameters);
        }

        // Pobieramy identyfikatory ról członka
        var roleIds = member.RoleIds;

        // Tworzymy zapytanie dla ról członka
        var query = _dbContext.Set<Role>()
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Where(r => r.OrganizationId == organizationId && roleIds.Contains(r.Id));

        // Zwracamy spaginowany wynik
        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }
}
