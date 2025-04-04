﻿using KeepItUp.MagJob.Identity.Core.Exceptions;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// Implementacja repozytorium użytkownika
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Inicjalizuje instancję repozytorium
    /// </summary>
    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            // Pobierz aktualną wersję użytkownika z bazy danych
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (existingUser == null)
            {
                throw new EntityNotFoundException($"User with ID {user.Id} not found.");
            }

            // Aktualizacja daty logowania
            if (user.LastLoginDate != existingUser.LastLoginDate)
            {
                existingUser.UpdateLastLoginDate(user.LastLoginDate);
            }

            // Aktualizacja statusu aktywności
            if (user.IsActive != existingUser.IsActive)
            {
                if (user.IsActive)
                {
                    existingUser.Activate();
                }
                else
                {
                    existingUser.Deactivate();
                }
            }

            // Pozostałe aktualizacje mogą być dodane tutaj w zależności od potrzeb
            // np. UpdateProfile, UpdatePermissions, itp.

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException($"User with ID {user.Id} has been modified by another user.");
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }
}
