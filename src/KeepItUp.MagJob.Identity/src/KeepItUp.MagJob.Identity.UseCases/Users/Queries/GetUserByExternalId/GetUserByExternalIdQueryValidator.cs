using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Validator for the GetUserByExternalIdQuery.
/// </summary>
public class GetUserByExternalIdQueryValidator : AbstractValidator<GetUserByExternalIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByExternalIdQueryValidator"/> class.
    /// </summary>
    public GetUserByExternalIdQueryValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotNull().WithMessage("Identyfikator zewnętrzny użytkownika jest wymagany.")
            .NotEqual(Guid.Empty).WithMessage("Identyfikator zewnętrzny użytkownika nie może być pusty.");
    }
}
