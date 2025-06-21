namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Walidator dla żądania GetUserOrganizationsRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class GetUserOrganizationsValidator : Validator<GetUserOrganizationsRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserOrganizationsValidator"/>.
    /// </summary>
    public GetUserOrganizationsValidator()
    {
        // Walidacja identyfikatora użytkownika
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");
    }
}
