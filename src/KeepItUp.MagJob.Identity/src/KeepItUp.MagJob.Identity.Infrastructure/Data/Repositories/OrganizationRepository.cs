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
}
