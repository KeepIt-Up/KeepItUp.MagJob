namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Validator for the CreateInvitationRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class CreateInvitationValidator : Validator<CreateInvitationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvitationValidator"/> class.
    /// </summary>
    public CreateInvitationValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres email jest wymagany.")
            .EmailAddress().WithMessage("Podany adres email jest nieprawidłowy.")
            .MaximumLength(100).WithMessage("Adres email nie może przekraczać 100 znaków.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");
    }
}