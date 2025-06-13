using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitations;

/// <summary>
/// Validator for the GetInvitationsQuery.
/// </summary>
public class GetInvitationsQueryValidator : AbstractValidator<GetInvitationsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationsQueryValidator"/> class.
    /// </summary>
    public GetInvitationsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
}