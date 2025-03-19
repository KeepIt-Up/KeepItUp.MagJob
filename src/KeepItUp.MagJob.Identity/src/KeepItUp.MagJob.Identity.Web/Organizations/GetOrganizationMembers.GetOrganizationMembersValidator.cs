namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania GetOrganizationMembersRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class GetOrganizationMembersValidator : Validator<GetOrganizationMembersRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationMembersValidator"/>.
    /// </summary>
    public GetOrganizationMembersValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
