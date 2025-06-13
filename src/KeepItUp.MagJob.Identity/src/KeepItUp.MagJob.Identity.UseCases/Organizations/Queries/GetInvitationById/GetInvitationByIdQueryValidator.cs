using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetInvitationById;

/// <summary>
/// Validator for the GetInvitationByIdQuery.
/// </summary>
public class GetInvitationByIdQueryValidator : AbstractValidator<GetInvitationByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationByIdQueryValidator"/> class.
    /// </summary>
    public GetInvitationByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Identyfikator zaproszenia jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}
