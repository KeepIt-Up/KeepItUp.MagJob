using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries.GetPermissions;

/// <summary>
/// Walidator dla zapytania GetPermissionsQuery.
/// </summary>
public class GetPermissionsQueryValidator : AbstractValidator<GetPermissionsQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetPermissionsQueryValidator"/>.
    /// </summary>
    public GetPermissionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
