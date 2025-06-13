namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Validator for the GetUserByIdRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class GetUserByIdValidator : Validator<GetUserByIdRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdValidator"/> class.
    /// </summary>
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");
    }
}
