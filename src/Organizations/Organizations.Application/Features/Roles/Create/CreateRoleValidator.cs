namespace Organizations.Application.Features.Roles.Create;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization Id is required");
    }
}