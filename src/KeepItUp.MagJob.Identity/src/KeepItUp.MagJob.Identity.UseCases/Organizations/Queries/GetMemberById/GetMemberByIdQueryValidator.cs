using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetMemberById;

/// <summary>
/// Walidator dla zapytania GetMemberByIdQuery.
/// </summary>
public class GetMemberByIdQueryValidator : AbstractValidator<GetMemberByIdQuery>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetMemberByIdQueryValidator"/>.
    /// </summary>
    public GetMemberByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika członka jest wymagany.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika wykonującego zapytanie jest wymagany.");
    }
} 
