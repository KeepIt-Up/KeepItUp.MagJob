using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.SharedKernel.Pagination;
using MediatR;
using Microsoft.Extensions.Logging;
namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizationsPaged;

/// <summary>
/// Handler dla zapytania GetUserOrganizationsPagedQuery.
/// </summary>
public class GetUserOrganizationsPagedQueryHandler : IRequestHandler<GetUserOrganizationsPagedQuery, Result<PaginationResult<OrganizationDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetUserOrganizationsPagedQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserOrganizationsPagedQueryHandler"/>.
    /// </summary>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetUserOrganizationsPagedQueryHandler(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        ILogger<GetUserOrganizationsPagedQueryHandler> logger)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetUserOrganizationsPagedQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetUserOrganizationsPagedQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Stronicowana lista organizacji, do których należy użytkownik.</returns>
    public async Task<Result<PaginationResult<OrganizationDto>>> Handle(GetUserOrganizationsPagedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userExists = await _userRepository.ExistsAsync(request.UserId, cancellationToken);
            if (!userExists)
            {
                return Result<PaginationResult<OrganizationDto>>.NotFound($"Użytkownik o identyfikatorze {request.UserId} nie istnieje.");
            }

            // Tworzymy wyrażenie do mapowania Organization na OrganizationDto
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

            // Używamy metody GetQueryableByUserId, która zwraca IQueryable<Organization>
            var result = await _organizationRepository.GetOrganizationsByUserIdAsync(request.UserId, selector, request.PaginationParameters, cancellationToken);



            return Result<PaginationResult<OrganizationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas pobierania organizacji użytkownika {UserId}", request.UserId);
            return Result<PaginationResult<OrganizationDto>>.Error("Wystąpił błąd podczas pobierania organizacji użytkownika.");
        }
    }
}
