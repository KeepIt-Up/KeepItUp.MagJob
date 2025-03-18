using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Komenda do aktualizacji logo organizacji.
/// </summary>
public record UpdateOrganizationLogoCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// URL do logo organizacji.
    /// </summary>
    public string LogoUrl { get; init; } = string.Empty;

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
}
