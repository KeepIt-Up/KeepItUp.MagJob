using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Handler for the GetUserByExternalIdQuery.
/// </summary>
public class GetUserByExternalIdQueryHandler : IRequestHandler<GetUserByExternalIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<GetUserByExternalIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByExternalIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public GetUserByExternalIdQueryHandler(
        IUserRepository repository,
        ILogger<GetUserByExternalIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserByExternalIdQuery.
    /// </summary>
    /// <param name="request">GetUserByExternalIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User data.</returns>
    public async Task<Result<UserDto>> Handle(GetUserByExternalIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _repository.GetByExternalIdAsync(request.ExternalId, cancellationToken);

            if (user == null)
            {
                return Result<UserDto>.NotFound($"Nie znaleziono użytkownika o identyfikatorze zewnętrznym {request.ExternalId}.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                ExternalId = user.ExternalId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                Memberships = user.Memberships?.Select(m => new MembershipDto
                {
                    MemberId = m.Id,
                    OrganizationId = m.OrganizationId,
                    JoinedAt = m.JoinedAt,
                    Roles = m.RoleIds.Select(r => r.ToString()).ToList()
                }).ToList() ?? new List<MembershipDto>()
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
            _logger.LogError(ex, "Błąd podczas pobierania użytkownika o identyfikatorze zewnętrznym {ExternalId}", request.ExternalId);
            return Result<UserDto>.Error("Wystąpił błąd podczas pobierania użytkownika: " + ex.Message);
        }
    }
}
