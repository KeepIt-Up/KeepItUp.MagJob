namespace Organizations.Application.Features.Invitations.Update;

public sealed class UpdateInvitationValidator : AbstractValidator<UpdateInvitationRequest>
{
    public UpdateInvitationValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty()
            .WithMessage("Invitation Id is required");

        RuleFor(x => x.status)
            .NotNull()
            .WithMessage("Invitation status is required");

        RuleFor(x => x.status)
            .IsInEnum()
            .WithMessage("Invalid invitation status");

        RuleFor(x => x.status)
            .NotEqual(InvitationStatus.Pending)
            .WithMessage("Invitation status cannot be pending");
    }
}