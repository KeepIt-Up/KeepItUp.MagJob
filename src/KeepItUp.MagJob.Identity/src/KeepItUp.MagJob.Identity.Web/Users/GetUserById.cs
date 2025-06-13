using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to get a user by their identifier.
/// </summary>
/// <remarks>
/// Gets a user by their identifier.
/// </remarks>
public class GetUserById : Endpoint<GetUserByIdRequest, GetUserByIdResponse>
{
    private readonly IMediator _mediator;
    private readonly IUserProfilePictureService _profilePictureService;
    private readonly ILogger<GetUserById> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserById"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="profilePictureService">Profile picture service.</param>
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
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserByIdRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetUser")
            .Produces<GetUserByIdResponse>(200)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Gets a user";
            s.Description = "Gets a user by their identifier";
            s.ExampleRequest = new GetUserByIdRequest { Id = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the GET /api/users/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the user data.</returns>
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

        // If the user does not have a profile picture, try to get it from the IDP
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
            ProfileImageUrl = profileImageUrl
        };

        await SendOkAsync(response, ct);
    }
}
