namespace KeepItUp.MagJob.Identity.Core.InvitationAggregate;

/// <summary>
/// Status of an invitation.
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    /// Invitation is pending acceptance.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Invitation has been accepted.
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Invitation has been rejected.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Invitation has expired.
    /// </summary>
    Expired = 3
}