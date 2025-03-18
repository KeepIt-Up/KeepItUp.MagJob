using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Walidator dla komendy CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateInvitationCommandValidator"/>.
    /// </summary>
    public CreateInvitationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
            .EmailAddress().WithMessage("Podany adres e-mail jest nieprawidłowy.")
            .MaximumLength(255).WithMessage("Adres e-mail nie może być dłuższy niż 255 znaków.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
