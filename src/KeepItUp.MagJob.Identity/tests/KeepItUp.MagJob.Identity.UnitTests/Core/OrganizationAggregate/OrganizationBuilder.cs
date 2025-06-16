using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;

/// <summary>
/// Builder pattern implementation for Organization aggregate.
/// Provides fluent API for creating Organization instances in tests.
/// </summary>
public class OrganizationBuilder
{
    private string _name = "Test Organization";
    private Guid _ownerId = Guid.NewGuid();
    private string? _description = "Test organization description";
    private string? _logoUrl = "https://example.com/logo.png";
    private string? _bannerUrl = "https://example.com/banner.png";
    private bool _isActive = true;
    private bool _withRoles = false;
    private bool _withOwnerMembership = false;

    /// <summary>
    /// Sets organization name.
    /// </summary>
    public OrganizationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets organization owner ID.
    /// </summary>
    public OrganizationBuilder WithOwner(Guid ownerId)
    {
        _ownerId = ownerId;
        return this;
    }

    /// <summary>
    /// Sets organization description.
    /// </summary>
    public OrganizationBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets organization logo URL.
    /// </summary>
    public OrganizationBuilder WithLogo(string? logoUrl)
    {
        _logoUrl = logoUrl;
        return this;
    }

    /// <summary>
    /// Sets organization banner URL.
    /// </summary>
    public OrganizationBuilder WithBanner(string? bannerUrl)
    {
        _bannerUrl = bannerUrl;
        return this;
    }

    /// <summary>
    /// Sets organization as active.
    /// </summary>
    public OrganizationBuilder AsActive()
    {
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Sets organization as inactive.
    /// </summary>
    public OrganizationBuilder AsInactive()
    {
        _isActive = false;
        return this;
    }

    /// <summary>
    /// Configures organization to initialize default roles.
    /// </summary>
    public OrganizationBuilder WithDefaultRoles()
    {
        _withRoles = true;
        return this;
    }

    /// <summary>
    /// Configures organization to initialize owner membership.
    /// Automatically includes default roles.
    /// </summary>
    public OrganizationBuilder WithOwnerMembership()
    {
        _withRoles = true;
        _withOwnerMembership = true;
        return this;
    }

    /// <summary>
    /// Creates minimal organization with only required fields.
    /// </summary>
    public OrganizationBuilder Minimal()
    {
        _description = null;
        _logoUrl = null;
        _bannerUrl = null;
        return this;
    }

    /// <summary>
    /// Creates organization with complete data.
    /// </summary>
    public OrganizationBuilder Complete()
    {
        _name = "Complete Organization";
        _description = "Complete organization with all optional fields";
        _logoUrl = "https://example.com/complete-logo.png";
        _bannerUrl = "https://example.com/complete-banner.png";
        return this;
    }

    /// <summary>
    /// Creates organization for edge case testing.
    /// </summary>
    public OrganizationBuilder ForEdgeCases()
    {
        _name = "A"; // Minimum length
        _description = "Edge case organization";
        return this;
    }

    /// <summary>
    /// Builds the Organization instance.
    /// </summary>
    public Organization Build()
    {
        var organization = Organization.Create(_name, _ownerId, _description, _logoUrl, _bannerUrl);

        // Set active/inactive status
        if (!_isActive)
        {
            organization.Deactivate();
        }

        // Initialize roles if requested
        if (_withRoles)
        {
            // Simulate having ID assigned by database
            typeof(Organization).GetProperty("Id")?.SetValue(organization, Guid.NewGuid());
            organization.InitializeRoles();

            // Initialize owner membership if requested
            if (_withOwnerMembership)
            {
                organization.InitializeOwner();
            }
        }

        return organization;
    }

    /// <summary>
    /// Creates a new OrganizationBuilder with default values.
    /// </summary>
    public static OrganizationBuilder New() => new OrganizationBuilder();

    /// <summary>
    /// Creates a new OrganizationBuilder with valid default values.
    /// </summary>
    public static OrganizationBuilder Valid() => new OrganizationBuilder();

    /// <summary>
    /// Creates multiple organizations using the current builder configuration.
    /// Each organization will have a unique owner ID and modified name.
    /// </summary>
    public List<Organization> BuildMany(int count)
    {
        var organizations = new List<Organization>();
        for (int i = 0; i < count; i++)
        {
            var builder = new OrganizationBuilder
            {
                _name = $"{_name} {i + 1}",
                _ownerId = Guid.NewGuid(),
                _description = _description != null ? $"{_description} {i + 1}" : null,
                _logoUrl = _logoUrl,
                _bannerUrl = _bannerUrl,
                _isActive = _isActive,
                _withRoles = _withRoles,
                _withOwnerMembership = _withOwnerMembership
            };
            organizations.Add(builder.Build());
        }
        return organizations;
    }
}