using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Walidator dla zapytania GetRolesByOrganizationIdQuery.
/// </summary>
public class GetRolesByOrganizationIdQueryValidator : AbstractValidator<GetRolesByOrganizationIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRolesByOrganizationIdQueryValidator"/>.
    /// </summary>
    public GetRolesByOrganizationIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
