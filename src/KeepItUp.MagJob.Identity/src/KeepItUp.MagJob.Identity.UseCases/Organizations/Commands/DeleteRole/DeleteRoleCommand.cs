using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;

/// <summary>
/// Komenda do usunięcia roli z organizacji.
/// </summary>
public record DeleteRoleCommand : IRequest<Result>
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
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
} 
