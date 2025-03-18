using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Walidator dla zapytania GetRolesByMemberIdQuery.
/// </summary>
public class GetRolesByMemberIdQueryValidator : AbstractValidator<GetRolesByMemberIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRolesByMemberIdQueryValidator"/>.
    /// </summary>
    public GetRolesByMemberIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika członka jest wymagany.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika wykonującego zapytanie jest wymagany.");
    }
}
