using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Validator for the GetRolesByOrganizationIdQuery.
/// </summary>
public class GetRolesByOrganizationIdQueryValidator : AbstractValidator<GetRolesByOrganizationIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetRolesByOrganizationIdQueryValidator"/> class.
    /// </summary>
    public GetRolesByOrganizationIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
