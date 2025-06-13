using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Handler for the GetOrganizationByIdQuery.
/// </summary>
public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, Result<OrganizationDto>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetOrganizationByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationByIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetOrganizationByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetOrganizationByIdQuery.
    /// </summary>
    /// <param name="request">GetOrganizationByIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Data of the organization.</returns>
    public async Task<Result<OrganizationDto>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<OrganizationDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            //bool hasAccess = organization.OwnerId == request.UserId ||
            //                 organization.Members.Any(m => m.UserId == request.UserId);

            //if (!hasAccess)
            //{
            //    return Result<OrganizationDto>.Forbidden("Brak dostępu do organizacji.");
            //}

            var userRoles = new List<string>();
            var member = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
            if (member != null)
            {
                userRoles = member.Roles.Select(r => r.Name).ToList();
            }
            else if (organization.OwnerId == request.UserId)
            {
                userRoles = organization.Roles.Select(r => r.Name).ToList();
            }

            var organizationDto = new OrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                OwnerId = organization.OwnerId,
                IsActive = organization.IsActive,
                UserRoles = userRoles,
                BannerUrl = organization.BannerUrl,
                LogoUrl = organization.LogoUrl
            };

            return Result<OrganizationDto>.Success(organizationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<OrganizationDto>.Error("Wystąpił błąd podczas pobierania organizacji: " + ex.Message);
        }
    }
}
