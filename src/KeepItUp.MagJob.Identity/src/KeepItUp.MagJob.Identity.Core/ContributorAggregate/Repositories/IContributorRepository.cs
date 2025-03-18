namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate.Repositories;

/// <summary>
/// Repozytorium dla encji Contributor
/// </summary>
public interface IContributorRepository
{
    /// <summary>
    /// Pobiera kontrybutora po ID
    /// </summary>
    Task<Contributor?> GetByIdAsync(Guid contributorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje kontrybutora
    /// </summary>
    Task<Contributor> AddAsync(Contributor contributor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje kontrybutora
    /// </summary>
    Task UpdateAsync(Contributor contributor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa kontrybutora
    /// </summary>
    Task DeleteAsync(Contributor contributor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Zwraca listę wszystkich kontrybutorów
    /// </summary>
    Task<List<Contributor>> ListAsync(CancellationToken cancellationToken = default);
}
