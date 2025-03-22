
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania organizacji po nazwie.
/// </summary>
public class OrganizationByNameSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationByNameSpec"/>.
    /// </summary>
    /// <param name="name">Nazwa organizacji do wyszukania.</param>
    public OrganizationByNameSpec(string name) =>
        Query
            .Where(org => org.Name == name);
} 
