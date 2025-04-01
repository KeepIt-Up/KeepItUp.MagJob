using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Walidator dla komendy DeactivateUserCommand.
/// </summary>
public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateUserCommandValidator"/>.
    /// </summary>
    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
