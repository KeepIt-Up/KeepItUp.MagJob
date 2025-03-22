using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Handler dla zapytania GetUserByExternalIdQuery.
/// </summary>
public class GetUserByExternalIdQueryHandler : IRequestHandler<GetUserByExternalIdQuery, Result<UserDto>>
{
    private readonly IReadRepository<User> _repository;
    private readonly ILogger<GetUserByExternalIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUserByExternalIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public GetUserByExternalIdQueryHandler(
        IReadRepository<User> repository,
        ILogger<GetUserByExternalIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetUserByExternalIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetUserByExternalIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Dane użytkownika.</returns>
    public async Task<Result<UserDto>> Handle(GetUserByExternalIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz użytkownika z repozytorium
            var user = await _repository.FirstOrDefaultAsync(
                new UserByExternalIdSpec(request.ExternalId), cancellationToken);

            if (user == null)
            {
                return Result<UserDto>.NotFound($"Nie znaleziono użytkownika o identyfikatorze zewnętrznym {request.ExternalId}.");
            }

            // Mapuj użytkownika na DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                ExternalId = user.ExternalId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            };

            // Mapuj profil użytkownika na DTO (jeśli istnieje)
            try
            {
                // Próbujemy uzyskać dostęp do właściwości profilu
                // Jeśli profil istnieje i ma dostępne właściwości, utworzymy DTO
                var phoneNumber = user.Profile?.PhoneNumber;
                var address = user.Profile?.Address;
                var profileImage = user.Profile?.ProfileImage;
                
                userDto.Profile = new UserProfileDto
                {
                    PhoneNumber = phoneNumber ?? string.Empty,
                    Address = address ?? string.Empty,
                    ProfileImageUrl = profileImage ?? string.Empty
                };
            }
            catch
            {
                // Jeśli wystąpi wyjątek, ustawiamy pusty profil
                userDto.Profile = new UserProfileDto
                {
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    ProfileImageUrl = string.Empty
                };
            }

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania użytkownika o identyfikatorze zewnętrznym {ExternalId}", request.ExternalId);
            return Result<UserDto>.Error("Wystąpił błąd podczas pobierania użytkownika: " + ex.Message);
        }
    }
} 
