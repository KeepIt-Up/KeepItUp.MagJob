using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Handler dla zapytania GetOrganizationMembersQuery.
/// </summary>
public class GetOrganizationMembersQueryHandler : IRequestHandler<GetOrganizationMembersQuery, Result<List<MemberDto>>>
{
    private readonly IReadRepository<Organization> _organizationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly ILogger<GetOrganizationMembersQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationMembersQueryHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationMembersQueryHandler(
        IReadRepository<Organization> organizationRepository,
        IReadRepository<User> userRepository,
        ILogger<GetOrganizationMembersQueryHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetOrganizationMembersQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetOrganizationMembersQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista członków organizacji.</returns>
    public async Task<Result<List<MemberDto>>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _organizationRepository.FirstOrDefaultAsync(
                new OrganizationWithMembersSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result<List<MemberDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.UserId ||
                             organization.Members.Any(m => m.UserId == request.UserId);

            if (!hasAccess)
            {
                return Result<List<MemberDto>>.Forbidden("Brak dostępu do organizacji.");
            }

            var result = new List<MemberDto>();

            // Pobierz dane użytkowników dla członków organizacji
            foreach (var member in organization.Members)
            {
                var user = await _userRepository.FirstOrDefaultAsync(
                    new UserByIdSpec(member.UserId), cancellationToken);

                if (user != null)
                {
                    var memberDto = new MemberDto
                    {
                        Id = member.Id,
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = member.Roles.Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            Color = r.Color,
                            Permissions = r.Permissions.Select(p => p.Name).ToList()
                        }).ToList()
                    };

                    result.Add(memberDto);
                }
            }

            return Result<List<MemberDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania członków organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<List<MemberDto>>.Error("Wystąpił błąd podczas pobierania członków organizacji: " + ex.Message);
        }
    }
} 
