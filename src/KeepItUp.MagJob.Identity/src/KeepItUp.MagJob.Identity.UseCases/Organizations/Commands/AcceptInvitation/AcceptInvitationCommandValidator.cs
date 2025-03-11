using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Walidator dla komendy AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AcceptInvitationCommandValidator"/>.
    /// </summary>
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("Identyfikator zaproszenia jest wymagany.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token zaproszenia jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
} 
