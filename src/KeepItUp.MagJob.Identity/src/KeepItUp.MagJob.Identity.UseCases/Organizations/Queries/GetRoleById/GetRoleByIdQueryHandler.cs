using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Handler for the GetRoleByIdQuery.
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetRoleByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetRoleByIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetRoleByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetRoleByIdQuery.
    /// </summary>
    /// <param name="request">GetRoleByIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Data of the role.</returns>
    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<RoleDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // bool hasAccess = organization.OwnerId == request.UserId ||
            //                  organization.Members.Any(m => m.UserId == request.UserId);

            // if (!hasAccess)
            // {
            //     return Result<RoleDto>.Forbidden("Brak dostępu do organizacji.");
            // }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result<RoleDto>.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Color = role.Color,
                Permissions = role.Permissions.Select(p => p.Name).ToList()
            };

            return Result<RoleDto>.Success(roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania roli o ID {RoleId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.OrganizationId);
            return Result<RoleDto>.Error("Wystąpił błąd podczas pobierania roli: " + ex.Message);
        }
    }
}
