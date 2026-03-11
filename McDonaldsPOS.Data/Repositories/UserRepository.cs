using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository for user-related data operations
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string pin)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() &&
                                     u.Pin == pin &&
                                     u.IsActive);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.DisplayName)
            .ToListAsync();
    }

    public async Task<bool> ValidateManagerPinAsync(string pin)
    {
        return await _dbSet
            .AnyAsync(u => u.Pin == pin && u.Role == UserRole.Manager && u.IsActive);
    }
}
