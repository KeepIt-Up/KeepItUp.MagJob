using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Validator for the GetOrganizationMembersQuery.
/// </summary>
public class GetOrganizationMembersQueryValidator : AbstractValidator<GetOrganizationMembersQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationMembersQueryValidator"/> class.
    /// </summary>
    public GetOrganizationMembersQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
