namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Walidator dla żądania UpdateUserRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class UpdateUserValidator : Validator<UpdateUserRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserValidator"/>.
    /// </summary>
    public UpdateUserValidator()
    {
        // Walidacja identyfikatora użytkownika
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");

        // Walidacja imienia
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może być dłuższe niż 100 znaków.");

        // Walidacja nazwiska
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(100).WithMessage("Nazwisko nie może być dłuższe niż 100 znaków.");
    }
}
