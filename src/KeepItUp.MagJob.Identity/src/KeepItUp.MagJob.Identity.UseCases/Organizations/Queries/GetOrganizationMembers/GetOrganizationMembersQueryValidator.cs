using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Walidator dla zapytania GetOrganizationMembersQuery.
/// </summary>
public class GetOrganizationMembersQueryValidator : AbstractValidator<GetOrganizationMembersQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationMembersQueryValidator"/>.
    /// </summary>
    public GetOrganizationMembersQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
} 
