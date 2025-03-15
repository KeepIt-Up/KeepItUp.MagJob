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
            .NotEmpty().WithMessage("Identyfikator zewnętrzny użytkownika jest wymagany.")
            .MaximumLength(100).WithMessage("Identyfikator zewnętrzny użytkownika nie może być dłuższy niż 100 znaków.");
    }
} 
