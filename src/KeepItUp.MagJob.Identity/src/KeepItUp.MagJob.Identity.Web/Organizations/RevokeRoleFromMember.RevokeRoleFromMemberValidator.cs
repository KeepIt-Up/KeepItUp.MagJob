namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the RevokeRoleFromMemberRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class RevokeRoleFromMemberValidator : Validator<RevokeRoleFromMemberRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeRoleFromMemberValidator"/> class.
    /// </summary>
    public RevokeRoleFromMemberValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");
    }
}
