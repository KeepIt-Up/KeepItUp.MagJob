namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Validator for the UpdateOrganizationRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class UpdateOrganizationValidator : Validator<UpdateOrganizationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationValidator"/> class.
    /// </summary>
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może przekraczać 100 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może przekraczać 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
