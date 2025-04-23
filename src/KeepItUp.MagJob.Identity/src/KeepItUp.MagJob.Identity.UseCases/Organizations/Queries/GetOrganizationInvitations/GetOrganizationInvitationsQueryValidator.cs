using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Walidator dla zapytania GetOrganizationInvitationsQuery.
/// </summary>
public class GetOrganizationInvitationsQueryValidator : AbstractValidator<GetOrganizationInvitationsQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationInvitationsQueryValidator"/>.
    /// </summary>
    public GetOrganizationInvitationsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
