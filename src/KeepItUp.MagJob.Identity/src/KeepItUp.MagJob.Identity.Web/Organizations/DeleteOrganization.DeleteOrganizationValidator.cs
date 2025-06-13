namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the DeleteOrganizationRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class DeleteOrganizationValidator : Validator<DeleteOrganizationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOrganizationValidator"/> class.
    /// </summary>
    public DeleteOrganizationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
