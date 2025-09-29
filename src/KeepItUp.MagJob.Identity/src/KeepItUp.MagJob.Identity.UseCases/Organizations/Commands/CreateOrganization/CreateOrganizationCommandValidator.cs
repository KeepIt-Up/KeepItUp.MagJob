using FluentValidation;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Validator for the CreateOrganizationCommand.
/// </summary>
public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrganizationCommandValidator"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    public CreateOrganizationCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może być dłuższa niż 100 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może być dłuższy niż 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Identyfikator właściciela organizacji jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");
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
