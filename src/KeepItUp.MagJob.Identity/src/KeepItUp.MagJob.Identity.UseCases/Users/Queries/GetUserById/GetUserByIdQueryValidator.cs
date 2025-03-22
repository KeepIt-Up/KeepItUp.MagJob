using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Walidator dla zapytania GetUserByIdQuery.
/// </summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserByIdQueryValidator"/>.
    /// </summary>
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
} 
