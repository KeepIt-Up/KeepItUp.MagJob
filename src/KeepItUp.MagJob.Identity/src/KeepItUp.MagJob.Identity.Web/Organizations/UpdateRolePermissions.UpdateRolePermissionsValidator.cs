namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the UpdateRolePermissionsRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class UpdateRolePermissionsValidator : Validator<UpdateRolePermissionsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRolePermissionsValidator"/> class.
    /// </summary>
    public UpdateRolePermissionsValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Lista uprawnień nie może być null.");

        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Nazwa uprawnienia nie może być pusta.")
            .MaximumLength(50).WithMessage("Nazwa uprawnienia nie może przekraczać 50 znaków.");
    }
}
