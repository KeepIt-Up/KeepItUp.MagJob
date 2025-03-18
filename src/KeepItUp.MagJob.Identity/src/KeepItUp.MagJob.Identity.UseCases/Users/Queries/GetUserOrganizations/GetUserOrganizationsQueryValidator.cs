using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Walidator dla zapytania GetUserOrganizationsQuery.
/// </summary>
public class GetUserOrganizationsQueryValidator : AbstractValidator<GetUserOrganizationsQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserOrganizationsQueryValidator"/>.
    /// </summary>
    public GetUserOrganizationsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
