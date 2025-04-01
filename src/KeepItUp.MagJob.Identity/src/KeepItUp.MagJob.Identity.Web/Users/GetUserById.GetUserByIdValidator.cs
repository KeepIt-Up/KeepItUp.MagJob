namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Walidator dla żądania GetUserByIdRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class GetUserByIdValidator : Validator<GetUserByIdRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserByIdValidator"/>.
    /// </summary>
    public GetUserByIdValidator()
    {
        // Walidacja identyfikatora użytkownika
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");
    }
}
