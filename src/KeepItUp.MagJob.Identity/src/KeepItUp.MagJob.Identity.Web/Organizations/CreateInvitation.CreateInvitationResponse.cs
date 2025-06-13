
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response containing the identifier of the created invitation.
/// </summary>
public class CreateInvitationResponse
{
    /// <summary>
    /// Identifier of the created invitation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the person being invited.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}