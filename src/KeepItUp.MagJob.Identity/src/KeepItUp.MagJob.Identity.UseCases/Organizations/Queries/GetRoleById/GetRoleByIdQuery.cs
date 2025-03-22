using Ardalis.Result;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Zapytanie o rolę na podstawie identyfikatora.
/// </summary>
public record GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
} 
