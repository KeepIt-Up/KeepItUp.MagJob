using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Validator for the GetUserOrganizationsQuery.
/// </summary>
public class GetUserOrganizationsQueryValidator : AbstractValidator<GetUserOrganizationsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserOrganizationsQueryValidator"/> class.
    /// </summary>
    public GetUserOrganizationsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
