using System.Security.Claims;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;
using Ardalis.Result;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania danych zalogowanego użytkownika.
/// </summary>
/// <remarks>
/// Pobiera dane użytkownika na podstawie tokenu JWT. Jeśli użytkownik nie istnieje w bazie danych,
/// automatycznie tworzy go na podstawie danych z tokenu JWT.
/// </remarks>
public class GetMe : EndpointWithoutRequest<GetUserByIdResponse>
{
    private readonly IMediator _mediator;
    private readonly IUserProfilePictureService _profilePictureService;
    private readonly ILogger<GetMe> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetMe"/>.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="profilePictureService">Serwis zdjęć profilowych.</param>
    /// <param name="logger">Logger.</param>
    public GetMe(
        IMediator mediator,
        IUserProfilePictureService profilePictureService,
        ILogger<GetMe> logger)
    {
        _mediator = mediator;
        _profilePictureService = profilePictureService;
        _logger = logger;
    }

    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/users/me");
        Description(b => b
            .WithName("GetMe")
            .Produces<GetUserByIdResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera dane zalogowanego użytkownika";
            s.Description = "Pobiera dane użytkownika na podstawie tokenu JWT. Jeśli użytkownik nie istnieje w bazie danych, automatycznie tworzy go na podstawie danych z tokenu JWT.";
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/identity/users/me.
    /// </summary>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi użytkownika.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        // Pobierz sub (identyfikator użytkownika) z tokenu
        var subClaim = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out var externalId))
        {
            _logger.LogWarning("Brak lub nieprawidłowy claim sub w tokenie JWT");
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserByExternalIdQuery
        {
            ExternalId = externalId
        };

        var result = await _mediator.Send(query, ct);

        // Jeśli użytkownik nie istnieje, utwórz go na podstawie danych z tokenu
        if (result.Status == ResultStatus.NotFound)
        {
            _logger.LogInformation("Użytkownik o ExternalId {ExternalId} nie istnieje w bazie danych. Tworzenie nowego użytkownika na podstawie tokenu JWT.", externalId);

            // Pobierz dane użytkownika z tokenu JWT
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? User.FindFirst("given_name")?.Value ?? string.Empty;
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? User.FindFirst("family_name")?.Value ?? string.Empty;
            var username = User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.FindFirst("preferred_username")?.Value
                ?? email
                ?? string.Empty;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Brak adresu email w tokenie JWT dla użytkownika {ExternalId}", externalId);
                await SendUnauthorizedAsync(ct);
                return;
            }

            // Utwórz użytkownika
            var createCommand = new CreateUserCommand
            {
                ExternalId = externalId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Username = username
            };

            var createResult = await _mediator.Send(createCommand, ct);

            if (createResult.Status == ResultStatus.Error)
            {
                _logger.LogError("Błąd podczas tworzenia użytkownika {ExternalId}: {Error}", externalId, createResult.Errors);
                await SendErrorsAsync(500, ct);
                return;
            }

            _logger.LogInformation("Utworzono nowego użytkownika {ExternalId} z ID {UserId}", externalId, createResult.Value);

            // Ponownie pobierz użytkownika po utworzeniu
            result = await _mediator.Send(query, ct);
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        if (result.Status == ResultStatus.NotFound)
        {
            _logger.LogError("Nie udało się pobrać użytkownika {ExternalId} po jego utworzeniu", externalId);
            await SendErrorsAsync(500, ct);
            return;
        }

        string? profileImageUrl = result.Value.ProfileImageUrl();

        // Jeśli użytkownik nie ma zdjęcia profilowego, spróbuj je pobrać z IDP
        if (string.IsNullOrEmpty(profileImageUrl))
        {
            try
            {
                profileImageUrl = await _profilePictureService.GetProfilePictureUrlAsync(
                    result.Value.Id,
                    result.Value.ExternalId,
                    true,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nie udało się pobrać zdjęcia profilowego użytkownika {ExternalId} z IDP", externalId);
                // Kontynuuj, nawet jeśli nie udało się pobrać zdjęcia
            }
        }

        var response = new GetUserByIdResponse
        {
            Id = result.Value.Id,
            ExternalId = result.Value.ExternalId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            IsActive = result.Value.IsActive,
            ProfileImageUrl = profileImageUrl,
            Memberships = result.Value.Memberships
        };

        await SendOkAsync(response, ct);
    }
}
