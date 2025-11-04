using Ardalis.Result;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Members.Queries.GetMemberEmail;

/// <summary>
/// Handler dla query pobierania emaila członka organizacji.
/// </summary>
public class GetMemberEmailHandler : IRequestHandler<GetMemberEmailQuery, Result<string>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetMemberEmailHandler> _logger;

    public GetMemberEmailHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<GetMemberEmailHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(GetMemberEmailQuery request, CancellationToken cancellationToken)
    {
        // Get organization by member ID
        var organization = await _organizationRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);
        
        if (organization == null)
        {
            _logger.LogWarning("Organization with member ID {MemberId} not found", request.MemberId);
            return Result.NotFound();
        }

        var member = organization.Members.FirstOrDefault(m => m.Id == request.MemberId);
        
        if (member == null)
        {
            _logger.LogWarning("Member with ID {MemberId} not found in organization", request.MemberId);
            return Result.NotFound();
        }

        var user = await _userRepository.GetByIdAsync(member.UserId, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for member {MemberId}", member.UserId, request.MemberId);
            return Result.NotFound();
        }

        return Result.Success(user.Email);
    }
}