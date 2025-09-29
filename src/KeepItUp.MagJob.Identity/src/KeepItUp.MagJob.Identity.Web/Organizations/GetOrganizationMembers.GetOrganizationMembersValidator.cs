namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the GetOrganizationMembersRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetOrganizationMembersValidator : Validator<GetOrganizationMembersRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationMembersValidator"/> class.
    /// </summary>
    public GetOrganizationMembersValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
