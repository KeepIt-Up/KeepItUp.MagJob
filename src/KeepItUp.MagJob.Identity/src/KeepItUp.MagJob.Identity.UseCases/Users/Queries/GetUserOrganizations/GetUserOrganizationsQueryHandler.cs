using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Handler dla zapytania GetUserOrganizationsQuery.
/// </summary>
public class GetUserOrganizationsQueryHandler : IRequestHandler<GetUserOrganizationsQuery, Result<List<OrganizationDto>>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IReadRepository<Organization> _organizationRepository;
    private readonly ILogger<GetUserOrganizationsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserOrganizationsQueryHandler"/>.
    /// </summary>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetUserOrganizationsQueryHandler(
        IReadRepository<User> userRepository,
        IReadRepository<Organization> organizationRepository,
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
    public async Task<Result<List<OrganizationDto>>> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź, czy użytkownik istnieje
            var user = await _userRepository.FirstOrDefaultAsync(
                new UserByIdSpec(request.UserId), cancellationToken);

            if (user == null)
            {
                return Result<List<OrganizationDto>>.NotFound($"Nie znaleziono użytkownika o ID {request.UserId}.");
            }

            // Pobierz organizacje, w których użytkownik jest członkiem
            var organizations = await _organizationRepository.ListAsync(
                new OrganizationsWithMemberSpec(request.UserId), cancellationToken);

            if (organizations.Count == 0)
            {
                return Result<List<OrganizationDto>>.Success(new List<OrganizationDto>());
            }

            var result = new List<OrganizationDto>();

            // Mapuj organizacje na DTO
            foreach (var organization in organizations)
            {
                var member = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                
                if (member != null)
                {
                    var organizationDto = new OrganizationDto
                    {
                        Id = organization.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        OwnerId = organization.OwnerId,
                        IsActive = organization.IsActive,
                        UserRoles = member.Roles.Select(r => r.Name).ToList()
                    };

                    result.Add(organizationDto);
                }
            }

            return Result<List<OrganizationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania organizacji dla użytkownika o ID {UserId}", request.UserId);
            return Result<List<OrganizationDto>>.Error("Wystąpił błąd podczas pobierania organizacji: " + ex.Message);
        }
    }
} 
