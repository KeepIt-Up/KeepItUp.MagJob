using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Komenda do dezaktywacji użytkownika.
/// </summary>
public record DeactivateUserCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; init; }
} 
