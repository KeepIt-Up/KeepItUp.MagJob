namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Reprezentuje uprawnienie w systemie.
/// </summary>
public class Permission
{
    /// <summary>
    /// Nazwa uprawnienia.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Opis uprawnienia.
    /// </summary>
    public string? Description { get; private set; }

    // Prywatny konstruktor dla EF Core
    private Permission() { }

    /// <summary>
    /// Tworzy nowe uprawnienie.
    /// </summary>
    /// <param name="name">Nazwa uprawnienia.</param>
    /// <param name="description">Opis uprawnienia.</param>
    public Permission(string name, string? description = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
        Description = description;
    }

    /// <summary>
    /// Standardowe uprawnienia w systemie.
    /// </summary>
    public static class StandardPermissions
    {
        // Uprawnienia dla organizacji
        public static readonly Permission ManageOrganization = new("organization.manage", "Zarządzanie organizacją");
        public static readonly Permission ViewOrganization = new("organization.view", "Przeglądanie organizacji");

        // Uprawnienia dla członków
        public static readonly Permission ManageMembers = new("members.manage", "Zarządzanie członkami organizacji");
        public static readonly Permission ViewMembers = new("members.view", "Przeglądanie członków organizacji");

        // Uprawnienia dla ról
        public static readonly Permission ManageRoles = new("roles.manage", "Zarządzanie rolami w organizacji");
        public static readonly Permission ViewRoles = new("roles.view", "Przeglądanie ról w organizacji");

        // Uprawnienia dla zaproszeń
        public static readonly Permission ManageInvitations = new("invitations.manage", "Zarządzanie zaproszeniami do organizacji");
        public static readonly Permission ViewInvitations = new("invitations.view", "Przeglądanie zaproszeń do organizacji");

        /// <summary>
        /// Pobiera wszystkie standardowe uprawnienia.
        /// </summary>
        /// <returns>Lista standardowych uprawnień.</returns>
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
