using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class CreateContributorValidator : Validator<CreateContributorRequest>
{
    public CreateContributorValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required.")
          .MinimumLength(2)
          .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
