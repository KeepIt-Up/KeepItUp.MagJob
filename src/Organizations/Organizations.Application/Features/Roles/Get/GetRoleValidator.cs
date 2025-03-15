namespace Organizations.Application.Features.Roles.Get;

public sealed class GetRoleValidator : AbstractValidator<GetRoleRequest>
{
    public GetRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}