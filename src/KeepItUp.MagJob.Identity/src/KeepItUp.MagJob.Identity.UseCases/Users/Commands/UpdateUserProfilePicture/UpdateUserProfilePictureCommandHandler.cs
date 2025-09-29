using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUserProfilePicture;

/// <summary>
/// Handler for the UpdateUserProfilePictureCommand.
/// </summary>
public class UpdateUserProfilePictureCommandHandler : IRequestHandler<UpdateUserProfilePictureCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private readonly ILogger<UpdateUserProfilePictureCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserProfilePictureCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="fileStorageService">File storage service.</param>
    /// <param name="fileValidationService">File validation service.</param>
    /// <param name="logger">Logger.</param>
    public UpdateUserProfilePictureCommandHandler(
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService,
        ILogger<UpdateUserProfilePictureCommandHandler> logger)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateUserProfilePictureCommand.
    /// </summary>
    /// <param name="request">UpdateUserProfilePictureCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the profile image URL.</returns>
    public async Task<Result<string>> Handle(UpdateUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate file
            _fileValidationService.ValidateImageFile(request.ProfilePictureFile, "profile picture");

            // Get user and validate permissions
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                return Result<string>.NotFound($"User with ID {request.UserId} not found.");
            }

            if (!user.IsActive)
            {
                return Result<string>.Error("Cannot update profile picture for inactive user.");
            }

            // Users can only update their own profile pictures (for now)
            // This can be extended to allow admin users to update other users' pictures
            if (request.UserId != request.CurrentUserId)
            {
                return Result<string>.Forbidden("User can only update their own profile picture.");
            }

            var oldProfileImageUrl = user.Profile?.ProfileImage;

            // Upload new profile picture
            string profileImageUrl;
            using (var stream = request.ProfilePictureFile.OpenReadStream())
            {
                profileImageUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    request.ProfilePictureFile.FileName,
                    request.ProfilePictureFile.ContentType,
                    "profile-pictures");
            }

            // Update user profile in database
            user.UpdateProfileProperties(profileImage: profileImageUrl);
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Delete old profile picture if exists and different
            if (!string.IsNullOrEmpty(oldProfileImageUrl) && oldProfileImageUrl != profileImageUrl)
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(oldProfileImageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old profile picture file {OldProfileImageUrl}", oldProfileImageUrl);
                    // Don't fail the operation if we can't delete the old file
                }
            }

            _logger.LogInformation("Successfully updated profile picture for user {UserId}", request.UserId);
            return Result.Success(profileImageUrl);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating profile picture for user {UserId}", request.UserId);
            return Result<string>.Error(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile picture for user {UserId}", request.UserId);
            return Result<string>.Error("Failed to update user profile picture: " + ex.Message);
        }
    }
}