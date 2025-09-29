using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUserProfilePicture;
using KeepItUp.MagJob.Identity.Web.Services;
using FluentValidation;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to update the profile picture of a user.
/// </summary>
public class UpdateUserProfilePicture : BaseEndpoint<UpdateUserProfilePictureRequest, UpdateUserProfilePictureResponse>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateUserProfilePicture> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserProfilePicture"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="currentUserAccessor">Current user accessor.</param>
    /// <param name="logger">Logger.</param>
    public UpdateUserProfilePicture(
        IMediator mediator,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateUserProfilePicture> logger)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateUserProfilePictureRequest.Route);
        AllowFileUploads();
        AllowFormData();
        Summary(s =>
        {
            s.Summary = "Updates the profile picture of a user";
            s.Description = "Updates the profile picture of a user with the specified identifier";
        });
    }

    /// <summary>
    /// Handles the PUT /api/users/{id}/profile-picture request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    protected override async Task<UpdateUserProfilePictureResponse> HandleEndpointAsync(UpdateUserProfilePictureRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetRequiredCurrentUserId();

        if (req.ProfilePictureFile == null || req.ProfilePictureFile.Length == 0)
        {
            throw new ValidationException("Profile picture file is required");
        }

        var command = new UpdateUserProfilePictureCommand
        {
            UserId = req.UserId,
            ProfilePictureFile = req.ProfilePictureFile,
            CurrentUserId = currentUserId
        };

        var result = await _mediator.Send(command, ct);

        return new UpdateUserProfilePictureResponse { ProfileImageUrl = result };
    }
}
