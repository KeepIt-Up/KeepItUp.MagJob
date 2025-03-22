using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania użytkownika po ID zewnętrznym (Keycloak).
/// </summary>
public class UserByExternalIdSpec : Specification<User>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserByExternalIdSpec"/>.
    /// </summary>
    /// <param name="externalId">ID zewnętrzne użytkownika do wyszukania.</param>
    public UserByExternalIdSpec(string externalId) =>
        Query
            .Where(user => user.ExternalId == externalId);
} 
