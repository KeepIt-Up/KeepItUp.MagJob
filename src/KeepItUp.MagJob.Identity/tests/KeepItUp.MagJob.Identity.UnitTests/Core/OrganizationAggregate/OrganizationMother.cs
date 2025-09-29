using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;

/// <summary>
/// Object Mother pattern implementation for Organization aggregate.
/// Provides pre-configured Organization instances for testing.
/// </summary>
public static class OrganizationMother
{
    /// <summary>
    /// Creates a basic valid organization with default values.
    /// </summary>
    public static Organization ValidOrganization() => Organization.Create(
        name: "Acme Corporation",
        ownerId: Guid.NewGuid(),
        description: "A leading technology company",
        logoUrl: "https://example.com/logo.png",
        bannerUrl: "https://example.com/banner.png");

    /// <summary>
    /// Creates an organization with custom name.
    /// </summary>
    public static Organization OrganizationWithName(string name) => Organization.Create(
        name: name,
        ownerId: Guid.NewGuid(),
        description: "Custom organization",
        logoUrl: "https://example.com/logo.png");

    /// <summary>
    /// Creates an organization with custom owner.
    /// </summary>
    public static Organization OrganizationWithOwner(Guid ownerId) => Organization.Create(
        name: "Test Organization",
        ownerId: ownerId,
        description: "Organization with specific owner");

    /// <summary>
    /// Creates a minimal organization with only required fields.
    /// </summary>
    public static Organization MinimalOrganization() => Organization.Create(
        name: "Minimal Org",
        ownerId: Guid.NewGuid());

    /// <summary>
    /// Creates an organization with complete data.
    /// </summary>
    public static Organization CompleteOrganization() => Organization.Create(
        name: "Complete Organization",
        ownerId: Guid.NewGuid(),
        description: "Complete organization with all optional fields",
        logoUrl: "https://example.com/complete-logo.png",
        bannerUrl: "https://example.com/complete-banner.png");

    /// <summary>
    /// Creates an inactive organization.
    /// </summary>
    public static Organization InactiveOrganization()
    {
        var organization = ValidOrganization();
        organization.Deactivate();
        return organization;
    }

    /// <summary>
    /// Creates an organization with initialized roles.
    /// Note: This requires the organization to have a valid ID.
    /// </summary>
    public static Organization OrganizationWithRoles()
    {
        var organization = ValidOrganization();
        // Simulate having ID assigned by database
        typeof(Organization).GetProperty("Id")?.SetValue(organization, Guid.NewGuid());
        organization.InitializeRoles();
        return organization;
    }

    /// <summary>
    /// Creates an organization with initialized roles and owner membership.
    /// </summary>
    public static Organization OrganizationWithOwnerMembership()
    {
        var organization = OrganizationWithRoles();
        organization.InitializeOwner();
        return organization;
    }

    /// <summary>
    /// Creates multiple organizations for batch testing.
    /// </summary>
    public static List<Organization> MultipleOrganizations(int count = 3)
    {
        var organizations = new List<Organization>();
        for (int i = 0; i < count; i++)
        {
            organizations.Add(Organization.Create(
                name: $"Organization {i + 1}",
                ownerId: Guid.NewGuid(),
                description: $"Test organization number {i + 1}"));
        }
        return organizations;
    }

    /// <summary>
    /// Creates an organization for testing edge cases.
    /// </summary>
    public static Organization OrganizationForEdgeCases() => Organization.Create(
        name: "A", // Minimum length name
        ownerId: Guid.NewGuid(),
        description: "Edge case organization with minimal name");

    /// <summary>
    /// Creates an organization with custom description.
    /// </summary>
    public static Organization OrganizationWithDescription(string description) => Organization.Create(
        name: "Test Organization",
        ownerId: Guid.NewGuid(),
        description: description);

    /// <summary>
    /// Creates an organization with custom logo URL.
    /// </summary>
    public static Organization OrganizationWithLogo(string logoUrl) => Organization.Create(
        name: "Logo Organization",
        ownerId: Guid.NewGuid(),
        logoUrl: logoUrl);

    /// <summary>
    /// Creates an organization with custom banner URL.
    /// </summary>
    public static Organization OrganizationWithBanner(string bannerUrl) => Organization.Create(
        name: "Banner Organization",
        ownerId: Guid.NewGuid(),
        bannerUrl: bannerUrl);
}