using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Walidator dla komendy RevokeRoleFromMemberCommand.
/// </summary>
public class RevokeRoleFromMemberCommandValidator : AbstractValidator<RevokeRoleFromMemberCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RevokeRoleFromMemberCommandValidator"/>.
    /// </summary>
    public RevokeRoleFromMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika wykonującego operację jest wymagany.");
    }
} 
