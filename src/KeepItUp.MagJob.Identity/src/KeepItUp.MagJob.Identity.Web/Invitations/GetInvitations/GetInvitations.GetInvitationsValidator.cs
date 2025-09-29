
namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Validator for the GetInvitationsRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetInvitationsValidator : Validator<GetInvitationsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationsValidator"/> class.
    /// </summary>
    public GetInvitationsValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
