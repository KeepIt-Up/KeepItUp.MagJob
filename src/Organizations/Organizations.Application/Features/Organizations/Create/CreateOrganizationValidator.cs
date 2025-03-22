namespace Organizations.Application.Features.Organizations.Create;

public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.description).MaximumLength(1000).WithMessage("Description must be less than 1000 characters");
    }
}