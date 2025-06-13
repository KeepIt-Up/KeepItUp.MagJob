using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Handler for the GetUserOrganizationsQuery.
/// </summary>
public class GetUserOrganizationsQueryHandler : IRequestHandler<GetUserOrganizationsQuery, Result<PaginationResult<OrganizationDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetUserOrganizationsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserOrganizationsQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetUserOrganizationsQueryHandler(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        ILogger<GetUserOrganizationsQueryHandler> logger)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserOrganizationsQuery.
    /// </summary>
    /// <param name="request">GetUserOrganizationsQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of organizations to which the user belongs.</returns>
    public async Task<Result<PaginationResult<OrganizationDto>>> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                return Result<PaginationResult<OrganizationDto>>.NotFound($"Nie znaleziono użytkownika o ID {request.UserId}.");
            }

            Expression<Func<Organization, OrganizationDto>> selector = org => new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                Description = org.Description,
                LogoUrl = org.LogoUrl,
                BannerUrl = org.BannerUrl,
                OwnerId = org.OwnerId,
                IsActive = org.IsActive,
                UserRoles = org.Members
                    .Where(m => m.UserId == request.UserId)
                    .SelectMany(m => m.Roles.Select(r => r.Name))
                    .ToList()
            };

            var result = await _organizationRepository.GetOrganizationsByUserIdAsync(request.UserId, selector, request.PaginationParameters, cancellationToken);

            return Result<PaginationResult<OrganizationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania organizacji dla użytkownika o ID {UserId}", request.UserId);
            return Result<PaginationResult<OrganizationDto>>.Error("Wystąpił błąd podczas pobierania organizacji: " + ex.Message);
        }
    }
}
