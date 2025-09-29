using FluentValidation;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Validator for the GetUserByIdQuery.
/// </summary>
/// <remarks>
/// Implements business validation, checking if the user exists in the database.
/// </remarks>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdQueryValidator"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    public GetUserByIdQueryValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");
    }

    /// <summary>
    /// Checks if the user with the given identifier exists.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True, if the user exists; otherwise false.</returns>
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }
}
