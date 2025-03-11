using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Walidator dla komendy RemoveMemberCommand.
/// </summary>
public class RemoveMemberCommandValidator : AbstractValidator<RemoveMemberCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RemoveMemberCommandValidator"/>.
    /// </summary>
    public RemoveMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika do usunięcia jest wymagany.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika wykonującego operację jest wymagany.");
    }
} 
