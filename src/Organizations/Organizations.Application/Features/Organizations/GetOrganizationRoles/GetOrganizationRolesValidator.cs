namespace Organizations.Application.Features.Organizations.GetOrganizationRoles;

public sealed class GetOrganizationRolesValidator : AbstractValidator<GetOrganizationRolesRequest>
{
    public GetOrganizationRolesValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Organization Id is required");
    }
}