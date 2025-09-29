namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Constants for organization permissions used in endpoint configuration.
/// </summary>
public static class OrganizationPermissions
{
    /// <summary>
    /// Permission to update the organization.
    /// </summary>
    public const string UpdateOrganization = "organization.manage";

    /// <summary>
    /// Permission to view the organization.
    /// </summary>
    public const string ViewOrganization = "organization.view";

    /// <summary>
    /// Permission to manage the members of the organization.
    /// </summary>
    public const string ManageMembers = "members.manage";

    /// <summary>
    /// Permission to view the members of the organization.
    /// </summary>
    public const string ViewMembers = "members.view";

    /// <summary>
    /// Permission to manage the roles of the organization.
    /// </summary>
    public const string ManageRoles = "roles.manage";

    /// <summary>
    /// Permission to view the roles of the organization.
    /// </summary>
    public const string ViewRoles = "roles.view";

    /// <summary>
    /// Permission to manage the invitations to the organization.
    /// </summary>
    public const string ManageInvitations = "invitations.manage";

    /// <summary>
    /// Permission to view the invitations to the organization.
    /// </summary>
    public const string ViewInvitations = "invitations.view";
}
