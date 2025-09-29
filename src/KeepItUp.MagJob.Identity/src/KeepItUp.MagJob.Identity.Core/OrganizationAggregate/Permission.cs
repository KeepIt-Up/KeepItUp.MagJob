namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Represents a permission in the system.
/// </summary>
public class Permission
{
    /// <summary>
    /// Name of the permission.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Description of the permission.
    /// </summary>
    public string? Description { get; private set; }

    // Private constructor for EF Core
    private Permission() { }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="name">Name of the permission.</param>
    /// <param name="description">Description of the permission.</param>
    public Permission(string name, string? description = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
        Description = description;
    }

    /// <summary>
    /// Standard permissions in the system.
    /// </summary>
    public static class StandardPermissions
    {
        public static readonly Permission ManageOrganization = new("organization.manage", "Zarządzanie organizacją");
        public static readonly Permission ViewOrganization = new("organization.view", "Przeglądanie organizacji");

        public static readonly Permission ManageMembers = new("members.manage", "Zarządzanie członkami organizacji");
        public static readonly Permission ViewMembers = new("members.view", "Przeglądanie członków organizacji");

        public static readonly Permission ManageRoles = new("roles.manage", "Zarządzanie rolami w organizacji");
        public static readonly Permission ViewRoles = new("roles.view", "Przeglądanie ról w organizacji");

        public static readonly Permission ManageInvitations = new("invitations.manage", "Zarządzanie zaproszeniami do organizacji");
        public static readonly Permission ViewInvitations = new("invitations.view", "Przeglądanie zaproszeń do organizacji");

        /// <summary>
        /// Gets all standard permissions.
        /// </summary>
        /// <returns>List of standard permissions.</returns>
        public static List<Permission> GetAll()
        {
            return new List<Permission>
            {
                ManageOrganization,
                ViewOrganization,
                ManageMembers,
                ViewMembers,
                ManageRoles,
                ViewRoles,
                ManageInvitations,
                ViewInvitations
            };
        }
    }
}
