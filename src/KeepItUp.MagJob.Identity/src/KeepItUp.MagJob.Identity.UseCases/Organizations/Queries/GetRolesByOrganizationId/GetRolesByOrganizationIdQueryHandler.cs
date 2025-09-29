using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Handler for the GetRolesByOrganizationIdQuery.
/// </summary>
public class GetRolesByOrganizationIdQueryHandler : IRequestHandler<GetRolesByOrganizationIdQuery, Result<PaginationResult<RoleDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetRolesByOrganizationIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetRolesByOrganizationIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetRolesByOrganizationIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetRolesByOrganizationIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetRolesByOrganizationIdQuery.
    /// </summary>
    /// <param name="request">GetRolesByOrganizationIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of roles in the organization with pagination.</returns>
    public async Task<Result<PaginationResult<RoleDto>>> Handle(GetRolesByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            // if (!hasAccess)
            // {
            //     return Result<PaginationResult<RoleDto>>.Forbidden("Brak dostępu do organizacji.");
            // }

            Expression<Func<Role, RoleDto>> selector = role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Color = role.Color,
                Permissions = role.Permissions.Select(p => p.Name).ToList()
            };

            var paginationResult = await _repository.GetRolesByOrganizationIdWithPaginationAsync(
                request.OrganizationId,
                selector,
                request.PaginationParameters,
                cancellationToken);

            return Result<PaginationResult<RoleDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<PaginationResult<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról organizacji: " + ex.Message);
        }
    }
}
