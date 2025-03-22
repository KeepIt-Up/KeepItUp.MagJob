

namespace Organizations.Application.Features.Organizations.CreateOrganizationInvitation;

public sealed class CreateOrganizationInvitationValidator : AbstractValidator<CreateOrganizationInvitationRequest>
{
    public CreateOrganizationInvitationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User Id is required");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization Id is required");
    }
}