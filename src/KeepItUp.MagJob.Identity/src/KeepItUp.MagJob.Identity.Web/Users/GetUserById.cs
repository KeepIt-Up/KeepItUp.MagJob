using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania użytkownika po identyfikatorze.
/// </summary>
/// <remarks>
/// Pobiera użytkownika o podanym identyfikatorze.
/// </remarks>
public class GetUserById : Endpoint<GetUserByIdRequest, GetUserByIdResponse>
{
    private readonly IMediator _mediator;
    private readonly IUserProfilePictureService _profilePictureService;
    private readonly ILogger<GetUserById> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserById"/>.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="profilePictureService">Serwis zdjęć profilowych.</param>
    /// <param name="logger">Logger.</param>
    public GetUserById(
        IMediator mediator,
        IUserProfilePictureService profilePictureService,
        ILogger<GetUserById> logger)
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
        Get(GetUserByIdRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetUser")
            .Produces<GetUserByIdResponse>(200)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera użytkownika";
            s.Description = "Pobiera użytkownika o podanym identyfikatorze";
            s.ExampleRequest = new GetUserByIdRequest { Id = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/users/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi użytkownika.</returns>
    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var query = new GetUserByIdQuery
        {
            Id = req.Id
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
                    false,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nie udało się pobrać zdjęcia profilowego użytkownika {UserId} z IDP", req.Id);
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
