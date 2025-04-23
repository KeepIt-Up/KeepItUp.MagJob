namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania RejectInvitationRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class RejectInvitationValidator : Validator<RejectInvitationRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RejectInvitationValidator"/>.
    /// </summary>
    public RejectInvitationValidator()
    {
        // Walidacja identyfikatora zaproszenia
        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("Identyfikator zaproszenia jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator zaproszenia nie może być pusty (Guid.Empty).");

        // Walidacja tokenu zaproszenia
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token zaproszenia jest wymagany.")
            .MaximumLength(256).WithMessage("Token zaproszenia nie może przekraczać 256 znaków.");
    }
}
