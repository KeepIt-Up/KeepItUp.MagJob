using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Validator for the GetOrganizationByIdQuery.
/// </summary>
public class GetOrganizationByIdQueryValidator : AbstractValidator<GetOrganizationByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationByIdQueryValidator"/> class.
    /// </summary>
    public GetOrganizationByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
