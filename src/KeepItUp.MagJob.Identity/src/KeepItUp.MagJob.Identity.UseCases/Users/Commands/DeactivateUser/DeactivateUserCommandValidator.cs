using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Validator for the DeactivateUserCommand.
/// </summary>
public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateUserCommandValidator"/> class.
    /// </summary>
    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
