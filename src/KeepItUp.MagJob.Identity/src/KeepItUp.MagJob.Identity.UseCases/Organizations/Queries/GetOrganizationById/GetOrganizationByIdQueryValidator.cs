using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Walidator dla zapytania GetOrganizationByIdQuery.
/// </summary>
public class GetOrganizationByIdQueryValidator : AbstractValidator<GetOrganizationByIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationByIdQueryValidator"/>.
    /// </summary>
    public GetOrganizationByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
