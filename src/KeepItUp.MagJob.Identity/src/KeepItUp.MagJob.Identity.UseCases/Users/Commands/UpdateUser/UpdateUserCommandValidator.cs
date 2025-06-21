using FluentValidation;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Walidator dla komendy UpdateUserCommand.
/// </summary>
/// <remarks>
/// Implementuje walidację biznesową, sprawdzając istnienie użytkownika w bazie danych.
/// </remarks>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserCommandValidator"/>.
    /// </summary>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

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
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("URL zdjęcia profilowego musi być prawidłowym adresem URL.");
    }

    /// <summary>
    /// Sprawdza, czy użytkownik o podanym identyfikatorze istnieje.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli użytkownik istnieje; w przeciwnym razie false.</returns>
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }
}
