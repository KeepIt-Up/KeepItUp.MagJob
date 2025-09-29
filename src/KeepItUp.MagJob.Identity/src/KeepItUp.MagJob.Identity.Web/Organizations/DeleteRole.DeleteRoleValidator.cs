namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the DeleteRoleRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class DeleteRoleValidator : Validator<DeleteRoleRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteRoleValidator"/> class.
    /// </summary>
    public DeleteRoleValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");
    }
}
