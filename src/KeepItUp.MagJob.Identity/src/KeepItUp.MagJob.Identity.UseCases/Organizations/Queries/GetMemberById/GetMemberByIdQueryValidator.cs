using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetMemberById;

/// <summary>
/// Validator for the GetMemberByIdQuery.
/// </summary>
public class GetMemberByIdQueryValidator : AbstractValidator<GetMemberByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetMemberByIdQueryValidator"/> class.
    /// </summary>
    public GetMemberByIdQueryValidator()
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
