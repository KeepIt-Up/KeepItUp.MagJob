using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.Core.ContributorAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// Implementacja repozytorium kontrybutorów
/// </summary>
public class ContributorRepository : IContributorRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Inicjalizuje instancję repozytorium
    /// </summary>
    public ContributorRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<Contributor?> GetByIdAsync(Guid contributorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Contributors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contributorId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Contributor> AddAsync(Contributor contributor, CancellationToken cancellationToken = default)
    {
        await _dbContext.Contributors.AddAsync(contributor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return contributor;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Contributor contributor, CancellationToken cancellationToken = default)
    {
        _dbContext.Contributors.Update(contributor);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Contributor contributor, CancellationToken cancellationToken = default)
    {
        _dbContext.Contributors.Remove(contributor);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Contributor>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Contributors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
