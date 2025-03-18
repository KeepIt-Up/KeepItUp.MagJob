using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Walidator dla komendy CreateOrganizationCommand.
/// </summary>
public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateOrganizationCommandValidator"/>.
    /// </summary>
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może być dłuższa niż 100 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może być dłuższy niż 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Identyfikator właściciela organizacji jest wymagany.");
    }
}
