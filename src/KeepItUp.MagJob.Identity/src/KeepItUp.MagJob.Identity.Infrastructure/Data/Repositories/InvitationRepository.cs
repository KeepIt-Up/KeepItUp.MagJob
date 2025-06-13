using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// Implementation of the invitation repository.
/// </summary>
public class InvitationRepository : IInvitationRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Initializes the repository instance.
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    public InvitationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<Invitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Invitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Token == token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Invitation>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Invitation>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .Where(i => i.Email == email)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Invitation>> GetPendingByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.Status == InvitationStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasPendingInvitationAsync(Guid organizationId, string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .AnyAsync(i => i.OrganizationId == organizationId &&
                          i.Email == email &&
                          i.Status == InvitationStatus.Pending &&
                          i.ExpiresAt > DateTime.UtcNow,
                      cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invitations
            .AsNoTracking()
            .AnyAsync(i => i.Id == invitationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Invitation> AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Invitations.AddAsync(invitation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return invitation;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _dbContext.Invitations.Update(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _dbContext.Invitations.Remove(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TDestination>> GetByOrganizationIdWithPaginationAsync<TDestination>(
        Expression<Func<Invitation, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        Expression<Func<Invitation, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Invitations
            .AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPaginationResultAsync(selector, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Invitation>> GetExpiredInvitationsAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;

        return await _dbContext.Invitations
            .Where(i => i.Status == InvitationStatus.Pending && i.ExpiresAt <= currentTime)
            .ToListAsync(cancellationToken);
    }
}