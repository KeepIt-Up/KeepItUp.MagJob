using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public CreateUserCommandHandler(
        IUserRepository repository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request">CreateUserCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of the created user.</returns>
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUserByEmail = await _repository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUserByEmail != null)
            {
                return Result<Guid>.Error("Użytkownik o podanym adresie e-mail już istnieje.");
            }

            var existingUserByExternalId = await _repository.GetByExternalIdAsync(request.ExternalId, cancellationToken);

            if (existingUserByExternalId != null)
            {
                return Result<Guid>.Error("Użytkownik o podanym identyfikatorze zewnętrznym już istnieje.");
            }

            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                string.IsNullOrEmpty(request.Username) ? request.Email : request.Username,
                request.ExternalId,
                true);

            if (!string.IsNullOrEmpty(request.PhoneNumber) ||
                !string.IsNullOrEmpty(request.Address) ||
                !string.IsNullOrEmpty(request.ProfileImageUrl))
            {
                user.UpdateProfile(
                    request.PhoneNumber,
                    request.Address,
                    request.ProfileImageUrl);
            }

            await _repository.AddAsync(user, cancellationToken);

            _logger.LogInformation("Utworzono nowego użytkownika o ID {UserId}", user.Id);

            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia użytkownika");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia użytkownika: " + ex.Message);
        }
    }
}
