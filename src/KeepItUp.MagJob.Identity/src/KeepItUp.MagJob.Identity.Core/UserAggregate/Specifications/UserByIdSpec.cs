using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania użytkownika po ID.
/// </summary>
public class UserByIdSpec : Specification<User>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserByIdSpec"/>.
    /// </summary>
    /// <param name="userId">ID użytkownika do wyszukania.</param>
    public UserByIdSpec(Guid userId) =>
        Query
            .Where(user => user.Id == userId);
} 
