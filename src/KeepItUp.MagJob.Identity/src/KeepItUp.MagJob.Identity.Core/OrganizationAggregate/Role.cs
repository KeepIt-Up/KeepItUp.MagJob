namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Represents a role in an organization.
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Name of the role.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Description of the role.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Color of the role (in HEX format).
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Organization ID to which the role belongs.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// List of permissions assigned to the role.
    /// </summary>
    private readonly List<Permission> _permissions = new();

    /// <summary>
    /// List of permissions assigned to the role (read-only).
    /// </summary>
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// List of members having this role.
    /// </summary>
    private readonly List<Member> _members = new();

    /// <summary>
    /// List of members having this role (read-only).
    /// </summary>
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();

    // Private constructor for EF Core
    private Role() { }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="name">Name of the role.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="description">Description of the role.</param>
    /// <param name="color">Color of the role (in HEX format).</param>
    /// <returns>New role.</returns>
    public static Role Create(string name, Guid organizationId, string? description = null, string? color = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.Default(organizationId, nameof(organizationId));

        return new Role
        {
            Name = name,
            OrganizationId = organizationId,
            Description = description,
            Color = color
        };
    }

    /// <summary>
    /// Updates the role data.
    /// </summary>
    /// <param name="name">Name of the role.</param>
    /// <param name="description">Description of the role.</param>
    /// <param name="color">Color of the role (in HEX format).</param>
    public void Update(string name, string? description = null, string? color = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
        Description = description;
        Color = color;
    }

    /// <summary>
    /// Adds a permission to the role.
    /// </summary>
    /// <param name="permission">Permission to add.</param>
    public void AddPermission(Permission permission)
    {
        Guard.Against.Null(permission, nameof(permission));

        // Check if the permission already exists
        if (_permissions.Any(p => p.Name == permission.Name))
        {
            return;
        }

        _permissions.Add(permission);
    }

    /// <summary>
    /// Removes a permission from the role.
    /// </summary>
    /// <param name="permissionName">Name of the permission to remove.</param>
    public void RemovePermission(string permissionName)
    {
        Guard.Against.NullOrEmpty(permissionName, nameof(permissionName));

        // Find the permission
        var permission = _permissions.FirstOrDefault(p => p.Name == permissionName);
        if (permission == null)
        {
            return;
        }

        _permissions.Remove(permission);
    }

    /// <summary>
    /// Checks if the role has a specific permission.
    /// </summary>
    /// <param name="permissionName">Name of the permission.</param>
    /// <returns>True, if the role has the permission; otherwise false.</returns>
    public bool HasPermission(string permissionName)
    {
        Guard.Against.NullOrEmpty(permissionName, nameof(permissionName));

        return _permissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Removes all permissions from the role.
    /// </summary>
    public void ClearPermissions()
    {
        _permissions.Clear();
    }
}
