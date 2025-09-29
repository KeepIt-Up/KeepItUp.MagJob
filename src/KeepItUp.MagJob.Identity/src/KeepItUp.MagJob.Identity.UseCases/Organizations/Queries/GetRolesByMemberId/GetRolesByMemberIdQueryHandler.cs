using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Handler for the GetRolesByMemberIdQuery.
/// </summary>
public class GetRolesByMemberIdQueryHandler : IRequestHandler<GetRolesByMemberIdQuery, Result<PaginationResult<RoleDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetRolesByMemberIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetRolesByMemberIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetRolesByMemberIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetRolesByMemberIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetRolesByMemberIdQuery.
    /// </summary>
    /// <param name="request">GetRolesByMemberIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of roles assigned to a member of an organization with pagination.</returns>
    public async Task<Result<PaginationResult<RoleDto>>> Handle(GetRolesByMemberIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.RequestingUserId, cancellationToken);

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

            var paginationResult = await _repository.GetRolesByMemberIdWithPaginationAsync(
                request.OrganizationId,
                request.MemberUserId,
                selector,
                request.PaginationParameters,
                cancellationToken);

            return Result<PaginationResult<RoleDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról członka o ID {MemberUserId} w organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);
            return Result<PaginationResult<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról członka: " + ex.Message);
        }
    }
}
