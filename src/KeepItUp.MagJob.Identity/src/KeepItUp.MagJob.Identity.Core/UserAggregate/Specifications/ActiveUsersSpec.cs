using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania aktywnych użytkowników.
/// </summary>
public class ActiveUsersSpec : Specification<User>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ActiveUsersSpec"/>.
    /// </summary>
    public ActiveUsersSpec() =>
        Query
            .Where(user => user.IsActive);
} 
