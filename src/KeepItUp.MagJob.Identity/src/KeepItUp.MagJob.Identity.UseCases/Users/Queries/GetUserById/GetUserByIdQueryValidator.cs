using FluentValidation;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Walidator dla zapytania GetUserByIdQuery.
/// </summary>
/// <remarks>
/// Implementuje walidację biznesową, sprawdzając istnienie użytkownika w bazie danych.
/// </remarks>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserByIdQueryValidator"/>.
    /// </summary>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public GetUserByIdQueryValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");
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
