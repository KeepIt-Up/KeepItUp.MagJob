using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;

/// <summary>
/// Walidator dla komendy DeactivateOrganizationCommand.
/// </summary>
public class DeactivateOrganizationCommandValidator : AbstractValidator<DeactivateOrganizationCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateOrganizationCommandValidator"/>.
    /// </summary>
    public DeactivateOrganizationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }
} 
