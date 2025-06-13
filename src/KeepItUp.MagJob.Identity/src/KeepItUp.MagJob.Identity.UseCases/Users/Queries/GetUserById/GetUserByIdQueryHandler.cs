using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Handler for the GetUserByIdQuery.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public GetUserByIdQueryHandler(
        IUserRepository repository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserByIdQuery.
    /// </summary>
    /// <param name="request">GetUserByIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User data.</returns>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return Result<UserDto>.NotFound($"Nie znaleziono użytkownika o ID {request.Id}.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                ExternalId = user.ExternalId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            };

            try
            {
                var phoneNumber = user.Profile?.PhoneNumber;
                var address = user.Profile?.Address;
                var profileImage = user.Profile?.ProfileImage;

                userDto.Profile = new UserProfileDto
                {
                    PhoneNumber = phoneNumber ?? string.Empty,
                    Address = address ?? string.Empty,
                    ProfileImageUrl = profileImage ?? string.Empty
                };
            }
            catch
            {
                userDto.Profile = new UserProfileDto
                {
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    ProfileImageUrl = string.Empty
                };
            }

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania użytkownika o ID {UserId}", request.Id);
            return Result<UserDto>.Error("Wystąpił błąd podczas pobierania użytkownika: " + ex.Message);
        }
    }
}
