using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Walidator dla zapytania GetRoleByIdQuery.
/// </summary>
public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRoleByIdQueryValidator"/>.
    /// </summary>
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
} 
