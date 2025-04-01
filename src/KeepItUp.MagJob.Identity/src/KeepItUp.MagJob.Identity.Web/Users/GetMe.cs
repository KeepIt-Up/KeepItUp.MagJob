using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania danych zalogowanego użytkownika.
/// </summary>
/// <remarks>
/// Pobiera dane użytkownika na podstawie tokenu JWT.
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
            s.Description = "Pobiera dane użytkownika na podstawie tokenu JWT";
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

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
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
            ProfileImageUrl = profileImageUrl
        };

        await SendOkAsync(response, ct);
    }
}
