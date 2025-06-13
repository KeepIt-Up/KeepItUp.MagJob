namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the GetOrganizationRolesRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetOrganizationRolesValidator : Validator<GetOrganizationRolesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationRolesValidator"/> class.
    /// </summary>
    public GetOrganizationRolesValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
