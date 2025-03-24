using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Handler dla komendy CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<Guid>>
{
    private readonly IRepository<Organization> _organizationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public CreateInvitationCommandHandler(
        IRepository<Organization> organizationRepository,
        IReadRepository<User> userRepository,
        ILogger<CreateInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę CreateInvitationCommand.
    /// </summary>
    /// <param name="request">Komenda CreateInvitationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonego zaproszenia.</returns>
    public async Task<Result<Guid>> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _organizationRepository.FirstOrDefaultAsync(
                new OrganizationWithRolesSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do tworzenia zaproszeń
            if (organization.OwnerId != request.UserId)
            {
                var isMember = organization.Members.Any(m => m.UserId == request.UserId && 
                    m.Roles.Any(r => r.Name == "Admin"));

                if (!isMember)
                {
                    return Result<Guid>.Forbidden("Brak uprawnień do tworzenia zaproszeń.");
                }
            }

            // Sprawdź, czy rola istnieje w organizacji
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            // Sprawdź, czy użytkownik o podanym adresie e-mail już istnieje
            var existingUser = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

            if (existingUser != null)
            {
                // Sprawdź, czy użytkownik jest już członkiem organizacji
                var isMember = organization.Members.Any(m => m.UserId == existingUser.Id);
                if (isMember)
                {
                    return Result<Guid>.Error("Użytkownik jest już członkiem organizacji.");
                }
            }

            // Sprawdź, czy istnieje aktywne zaproszenie dla tego adresu e-mail
            var existingInvitation = organization.Invitations
                .FirstOrDefault(i => i.Email == request.Email && !i.IsExpired && 
                                     i.Status == InvitationStatus.Pending);

            if (existingInvitation != null)
            {
                return Result<Guid>.Error("Istnieje już aktywne zaproszenie dla tego adresu e-mail.");
            }

            // Utwórz nowe zaproszenie
            var invitation = organization.CreateInvitation(request.Email, request.RoleId);

            // Zapisz zmiany w repozytorium
            await _organizationRepository.UpdateAsync(organization, cancellationToken);
            await _organizationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono zaproszenie o ID {InvitationId} dla adresu e-mail {Email} do organizacji {OrganizationId}",
                invitation.Id, request.Email, organization.Id);

            return Result<Guid>.Success(invitation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia zaproszenia");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia zaproszenia: " + ex.Message);
        }
    }
} 
