using FluentValidation;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUserProfilePicture;

/// <summary>
/// Validator for the UpdateUserProfilePictureCommand.
/// </summary>
public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserProfilePictureCommandValidator"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    public UpdateUserProfilePictureCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User identifier is required.")
            .MustAsync(UserExists).WithMessage("User with the given identifier does not exist.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("Current user identifier is required.")
            .MustAsync(UserExists).WithMessage("Current user with the given identifier does not exist.");

        RuleFor(x => x.ProfilePictureFile)
            .NotNull().WithMessage("Profile picture file is required.");

        // Business rule: Users can only update their own profile pictures
        RuleFor(x => x)
            .Must(command => command.UserId == command.CurrentUserId)
            .WithMessage("Users can only update their own profile pictures.");
    }

    /// <summary>
    /// Checks if a user with the given identifier exists.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user exists; otherwise false.</returns>
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }
}