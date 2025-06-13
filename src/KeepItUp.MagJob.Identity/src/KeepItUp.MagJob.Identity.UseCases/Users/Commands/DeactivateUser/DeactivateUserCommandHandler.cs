using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Handler for the DeactivateUserCommand.
/// </summary>
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public DeactivateUserCommandHandler(
        IUserRepository repository,
        ILogger<DeactivateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeactivateUserCommand.
    /// </summary>
    /// <param name="request">DeactivateUserCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return Result.NotFound($"Nie znaleziono użytkownika o ID {request.Id}.");
            }

            user.Deactivate();

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Dezaktywowano użytkownika o ID {UserId}", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dezaktywacji użytkownika");
            return Result.Error("Wystąpił błąd podczas dezaktywacji użytkownika: " + ex.Message);
        }
    }
}
