using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository interface for user operations
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> ValidateCredentialsAsync(string username, string pin);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> ValidateManagerPinAsync(string pin);
}
