namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Validator for the UpdateUserRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class UpdateUserValidator : Validator<UpdateUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserValidator"/> class.
    /// </summary>
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może być dłuższe niż 100 znaków.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(100).WithMessage("Nazwisko nie może być dłuższe niż 100 znaków.");
    }
}
