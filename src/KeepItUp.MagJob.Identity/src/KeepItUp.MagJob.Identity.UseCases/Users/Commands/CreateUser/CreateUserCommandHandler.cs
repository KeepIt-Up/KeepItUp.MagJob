using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;

/// <summary>
/// Handler dla komendy CreateUserCommand.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateUserCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public CreateUserCommandHandler(
        IUserRepository repository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę CreateUserCommand.
    /// </summary>
    /// <param name="request">Komenda CreateUserCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonego użytkownika.</returns>
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź, czy użytkownik o podanym adresie e-mail już istnieje
            var existingUserByEmail = await _repository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUserByEmail != null)
            {
                return Result<Guid>.Error("Użytkownik o podanym adresie e-mail już istnieje.");
            }

            // Sprawdź, czy użytkownik o podanym identyfikatorze zewnętrznym już istnieje
            var existingUserByExternalId = await _repository.GetByExternalIdAsync(request.ExternalId, cancellationToken);

            if (existingUserByExternalId != null)
            {
                return Result<Guid>.Error("Użytkownik o podanym identyfikatorze zewnętrznym już istnieje.");
            }

            // Utwórz nowego użytkownika
            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                string.IsNullOrEmpty(request.Username) ? request.Email : request.Username,
                request.ExternalId,
                true); // Domyślnie użytkownik jest aktywny

            // Aktualizuj profil użytkownika (jeśli podano dane profilu)
            if (!string.IsNullOrEmpty(request.PhoneNumber) ||
                !string.IsNullOrEmpty(request.Address) ||
                !string.IsNullOrEmpty(request.ProfileImageUrl))
            {
                user.UpdateProfile(
                    request.PhoneNumber,
                    request.Address,
                    request.ProfileImageUrl);
            }

            // Zapisz użytkownika w repozytorium
            await _repository.AddAsync(user, cancellationToken);

            _logger.LogInformation("Utworzono nowego użytkownika o ID {UserId}", user.Id);

            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia użytkownika");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia użytkownika: " + ex.Message);
        }
    }
}
