using FluentValidation;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class GetContributorValidator : Validator<GetContributorByIdRequest>
{
    public GetContributorValidator()
    {
        RuleFor(x => x.ContributorId)
          .NotNull().WithMessage("Id is required");
    }
}
