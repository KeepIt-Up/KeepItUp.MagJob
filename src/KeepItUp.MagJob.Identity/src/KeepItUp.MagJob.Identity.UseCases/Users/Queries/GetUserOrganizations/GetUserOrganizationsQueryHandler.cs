using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Handler dla zapytania GetUserOrganizationsQuery.
/// </summary>
public class GetUserOrganizationsQueryHandler : IRequestHandler<GetUserOrganizationsQuery, Result<PaginationResult<OrganizationDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetUserOrganizationsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserOrganizationsQueryHandler"/>.
    /// </summary>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
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
    /// Obsługuje zapytanie GetUserOrganizationsQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetUserOrganizationsQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista organizacji, do których należy użytkownik.</returns>
    public async Task<Result<PaginationResult<OrganizationDto>>> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź, czy użytkownik istnieje
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                return Result<PaginationResult<OrganizationDto>>.NotFound($"Nie znaleziono użytkownika o ID {request.UserId}.");
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
            _logger.LogError(ex, "Błąd podczas pobierania organizacji dla użytkownika o ID {UserId}", request.UserId);
            return Result<PaginationResult<OrganizationDto>>.Error("Wystąpił błąd podczas pobierania organizacji: " + ex.Message);
        }
    }
}
