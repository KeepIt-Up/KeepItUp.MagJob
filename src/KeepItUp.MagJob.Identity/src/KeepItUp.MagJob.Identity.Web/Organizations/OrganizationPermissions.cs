using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Stałe dla uprawnień organizacji używane w konfiguracji endpointów.
/// </summary>
public static class OrganizationPermissions
{
    /// <summary>
    /// Uprawnienie do aktualizacji organizacji.
    /// </summary>
    public const string UpdateOrganization = "organization.manage";

    /// <summary>
    /// Uprawnienie do przeglądania organizacji.
    /// </summary>
    public const string ViewOrganization = "organization.view";

    /// <summary>
    /// Uprawnienie do zarządzania członkami organizacji.
    /// </summary>
    public const string ManageMembers = "members.manage";

    /// <summary>
    /// Uprawnienie do przeglądania członków organizacji.
    /// </summary>
    public const string ViewMembers = "members.view";

    /// <summary>
    /// Uprawnienie do zarządzania rolami w organizacji.
    /// </summary>
    public const string ManageRoles = "roles.manage";

    /// <summary>
    /// Uprawnienie do przeglądania ról w organizacji.
    /// </summary>
    public const string ViewRoles = "roles.view";

    /// <summary>
    /// Uprawnienie do zarządzania zaproszeniami do organizacji.
    /// </summary>
    public const string ManageInvitations = "invitations.manage";

    /// <summary>
    /// Uprawnienie do przeglądania zaproszeń do organizacji.
    /// </summary>
    public const string ViewInvitations = "invitations.view";
}
