namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the RejectInvitationRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class RejectInvitationValidator : Validator<RejectInvitationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RejectInvitationValidator"/> class.
    /// </summary>
    public RejectInvitationValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("Identyfikator zaproszenia jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator zaproszenia nie może być pusty (Guid.Empty).");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token zaproszenia jest wymagany.")
            .MaximumLength(256).WithMessage("Token zaproszenia nie może przekraczać 256 znaków.");
    }
}
