using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Handler for the UpdateUserCommand.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public UpdateUserCommandHandler(
        IUserRepository repository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateUserCommand.
    /// </summary>
    /// <param name="request">UpdateUserCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return Result.NotFound($"Nie znaleziono użytkownika o ID {request.Id}.");
            }

            user.Update(request.FirstName, request.LastName);

            user.UpdateProfile(
                request.PhoneNumber,
                request.Address,
                request.ProfileImageUrl);

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Zaktualizowano użytkownika o ID {UserId}", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji użytkownika");
            return Result.Error("Wystąpił błąd podczas aktualizacji użytkownika: " + ex.Message);
        }
    }
}
