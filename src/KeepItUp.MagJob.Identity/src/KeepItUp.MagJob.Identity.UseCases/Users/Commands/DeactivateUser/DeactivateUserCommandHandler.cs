using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Handler dla komendy DeactivateUserCommand.
/// </summary>
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IRepository<User> _repository;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateUserCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public DeactivateUserCommandHandler(
        IRepository<User> repository,
        ILogger<DeactivateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę DeactivateUserCommand.
    /// </summary>
    /// <param name="request">Komenda DeactivateUserCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz użytkownika z repozytorium
            var user = await _repository.FirstOrDefaultAsync(
                new UserByIdSpec(request.Id), cancellationToken);

            if (user == null)
            {
                return Result.NotFound($"Nie znaleziono użytkownika o ID {request.Id}.");
            }

            // Dezaktywuj użytkownika
            user.Deactivate();

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Dezaktywowano użytkownika o ID {UserId}", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dezaktywacji użytkownika");
            return Result.Error("Wystąpił błąd podczas dezaktywacji użytkownika: " + ex.Message);
        }
    }
} 
