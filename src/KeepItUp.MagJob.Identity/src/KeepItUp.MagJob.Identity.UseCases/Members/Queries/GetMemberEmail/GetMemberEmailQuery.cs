using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Members.Queries.GetMemberEmail;

/// <summary>
/// Query do pobierania emaila członka organizacji.
/// </summary>
public class GetMemberEmailQuery : IRequest<Result<string>>
{
    /// <summary>
    /// Identyfikator członka.
    /// </summary>
    public Guid MemberId { get; set; }
}