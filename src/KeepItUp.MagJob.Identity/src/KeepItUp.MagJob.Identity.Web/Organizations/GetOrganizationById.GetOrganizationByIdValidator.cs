namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania GetOrganizationByIdRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class GetOrganizationByIdValidator : Validator<GetOrganizationByIdRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationByIdValidator"/>.
    /// </summary>
    public GetOrganizationByIdValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
