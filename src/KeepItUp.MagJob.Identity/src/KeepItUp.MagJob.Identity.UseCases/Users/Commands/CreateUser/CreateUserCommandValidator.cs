using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;

/// <summary>
/// Walidator dla komendy CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateUserCommandValidator"/>.
    /// </summary>
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("Identyfikator zewnętrzny jest wymagany.")
            .MaximumLength(100).WithMessage("Identyfikator zewnętrzny nie może być dłuższy niż 100 znaków.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
            .EmailAddress().WithMessage("Podany adres e-mail jest nieprawidłowy.")
            .MaximumLength(255).WithMessage("Adres e-mail nie może być dłuższy niż 255 znaków.");

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
