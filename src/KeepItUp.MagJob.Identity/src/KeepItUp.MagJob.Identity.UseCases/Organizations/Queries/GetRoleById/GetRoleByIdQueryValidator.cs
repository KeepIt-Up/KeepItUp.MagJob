using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Validator for the GetRoleByIdQuery.
/// </summary>
public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetRoleByIdQueryValidator"/> class.
    /// </summary>
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
