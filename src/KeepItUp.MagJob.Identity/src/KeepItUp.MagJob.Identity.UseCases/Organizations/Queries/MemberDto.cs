namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// DTO for a member of an organization.
/// </summary>
public class MemberDto
{
    /// <summary>
    /// Member identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Date of joining the organization.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Roles assigned to the member.
    /// </summary>
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
}
