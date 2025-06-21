using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Walidator dla zapytania GetUserByExternalIdQuery.
/// </summary>
public class GetUserByExternalIdQueryValidator : AbstractValidator<GetUserByExternalIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserByExternalIdQueryValidator"/>.
    /// </summary>
    public GetUserByExternalIdQueryValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotNull().WithMessage("Identyfikator zewnętrzny użytkownika jest wymagany.")
            .NotEqual(Guid.Empty).WithMessage("Identyfikator zewnętrzny użytkownika nie może być pusty.");
    }
}
