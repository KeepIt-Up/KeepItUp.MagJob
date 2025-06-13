namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the GetOrganizationByIdRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetOrganizationByIdValidator : Validator<GetOrganizationByIdRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationByIdValidator"/> class.
    /// </summary>
    public GetOrganizationByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
