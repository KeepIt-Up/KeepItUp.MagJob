using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Handler dla komendy UpdateUserCommand.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public UpdateUserCommandHandler(
        IUserRepository repository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę UpdateUserCommand.
    /// </summary>
    /// <param name="request">Komenda UpdateUserCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz użytkownika z repozytorium
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return Result.NotFound($"Nie znaleziono użytkownika o ID {request.Id}.");
            }

            // Aktualizuj podstawowe dane użytkownika
            user.Update(request.FirstName, request.LastName);

            // Aktualizuj profil użytkownika
            user.UpdateProfile(
                request.PhoneNumber,
                request.Address,
                request.ProfileImageUrl);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Zaktualizowano użytkownika o ID {UserId}", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji użytkownika");
            return Result.Error("Wystąpił błąd podczas aktualizacji użytkownika: " + ex.Message);
        }
    }
}
