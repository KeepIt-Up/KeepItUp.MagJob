using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Walidator dla komendy RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RejectInvitationCommandValidator"/>.
    /// </summary>
    public RejectInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("Identyfikator zaproszenia jest wymagany.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token zaproszenia jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
