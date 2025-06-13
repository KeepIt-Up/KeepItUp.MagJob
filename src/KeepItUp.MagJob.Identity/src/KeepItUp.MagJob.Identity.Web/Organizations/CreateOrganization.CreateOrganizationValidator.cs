namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the CreateOrganizationRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class CreateOrganizationValidator : Validator<CreateOrganizationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrganizationValidator"/> class.
    /// </summary>
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może przekraczać 100 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może przekraczać 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
