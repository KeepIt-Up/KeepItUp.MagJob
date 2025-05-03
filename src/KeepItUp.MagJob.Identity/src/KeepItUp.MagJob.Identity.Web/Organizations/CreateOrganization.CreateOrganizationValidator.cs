namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania CreateOrganizationRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class CreateOrganizationValidator : Validator<CreateOrganizationRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateOrganizationValidator"/>.
    /// </summary>
    public CreateOrganizationValidator()
    {
        // Walidacja nazwy organizacji
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może przekraczać 100 znaków.");

        // Walidacja opisu organizacji (opcjonalny)
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może przekraczać 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
