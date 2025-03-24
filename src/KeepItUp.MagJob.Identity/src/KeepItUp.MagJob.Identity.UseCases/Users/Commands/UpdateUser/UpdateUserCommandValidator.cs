using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Walidator dla komendy UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserCommandValidator"/>.
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może być dłuższe niż 100 znaków.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(100).WithMessage("Nazwisko nie może być dłuższe niż 100 znaków.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Numer telefonu nie może być dłuższy niż 20 znaków.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Adres nie może być dłuższy niż 255 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(1000).WithMessage("URL zdjęcia profilowego nie może być dłuższy niż 1000 znaków.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("URL zdjęcia profilowego musi być prawidłowym adresem URL.")
            .When(x => !string.IsNullOrEmpty(x.ProfileImageUrl));
    }
} 
