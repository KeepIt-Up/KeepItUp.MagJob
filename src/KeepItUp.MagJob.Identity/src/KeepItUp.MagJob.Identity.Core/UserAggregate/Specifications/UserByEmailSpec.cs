using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania użytkownika po adresie email.
/// </summary>
public class UserByEmailSpec : Specification<User>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserByEmailSpec"/>.
    /// </summary>
    /// <param name="email">Adres email użytkownika do wyszukania.</param>
    public UserByEmailSpec(string email) =>
        Query
            .Where(user => user.Email == email);
} 
