using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetInvitationById;

/// <summary>
/// Walidator dla zapytania GetInvitationByIdQuery.
/// </summary>
public class GetInvitationByIdQueryValidator : AbstractValidator<GetInvitationByIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetInvitationByIdQueryValidator"/>.
    /// </summary>
    public GetInvitationByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Identyfikator zaproszenia jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
