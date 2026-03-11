using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    bool IsManager { get; }

    Task<(bool Success, string Message)> LoginAsync(string username, string pin);
    void Logout();
    Task<bool> ValidateManagerPinAsync(string pin);
    Task<IEnumerable<User>> GetActiveUsersAsync();

    event EventHandler<User?>? UserChanged;
}
