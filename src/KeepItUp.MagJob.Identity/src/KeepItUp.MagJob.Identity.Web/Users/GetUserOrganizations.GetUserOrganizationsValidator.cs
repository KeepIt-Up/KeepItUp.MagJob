namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Validator for the GetUserOrganizationsRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetUserOrganizationsValidator : Validator<GetUserOrganizationsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserOrganizationsValidator"/> class.
    /// </summary>
    public GetUserOrganizationsValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");
    }
}
