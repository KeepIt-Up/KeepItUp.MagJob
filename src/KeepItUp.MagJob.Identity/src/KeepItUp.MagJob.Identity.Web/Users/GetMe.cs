using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to get the data of the logged in user.
/// </summary>
/// <remarks>
/// Gets the user data based on the JWT token.
/// </remarks>
public class GetMe : EndpointWithoutRequest<GetUserByIdResponse>
{
    private readonly IMediator _mediator;
    private readonly IUserProfilePictureService _profilePictureService;
    private readonly ILogger<GetMe> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMe"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="profilePictureService">Profile picture service.</param>
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
    /// Configures the endpoint.
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
    /// Handles the GET /api/identity/users/me request.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the user data.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        // Get sub (user identifier) from the token
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

        // If the user does not have a profile picture, try to get it from the IDP
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
                // Continue even if the profile picture could not be retrieved
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
