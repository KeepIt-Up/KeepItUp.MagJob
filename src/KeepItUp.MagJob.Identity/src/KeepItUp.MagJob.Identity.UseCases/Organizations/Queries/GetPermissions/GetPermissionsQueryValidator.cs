using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

/// <summary>
/// Validator for the GetPermissionsQuery.
/// </summary>
public class GetPermissionsQueryValidator : AbstractValidator<GetPermissionsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPermissionsQueryValidator"/> class.
    /// </summary>
    public GetPermissionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
