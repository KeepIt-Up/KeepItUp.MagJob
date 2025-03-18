using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;

/// <summary>
/// Walidator dla komendy AssignRoleToMemberCommand.
/// </summary>
public class AssignRoleToMemberCommandValidator : AbstractValidator<AssignRoleToMemberCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AssignRoleToMemberCommandValidator"/>.
    /// </summary>
    public AssignRoleToMemberCommandValidator()
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
