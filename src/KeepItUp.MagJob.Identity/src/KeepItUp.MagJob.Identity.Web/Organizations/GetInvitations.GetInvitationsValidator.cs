namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania GetInvitationsRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class GetInvitationsValidator : Validator<GetInvitationsRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetInvitationsValidator"/>.
    /// </summary>
    public GetInvitationsValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
