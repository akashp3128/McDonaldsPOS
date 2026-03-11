using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Data.Repositories;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service for authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    public bool IsManager => _currentUser?.Role == UserRole.Manager;

    public event EventHandler<User?>? UserChanged;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool Success, string Message)> LoginAsync(string username, string pin)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required");

        if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
            return (false, "PIN must be 4 digits");

        var user = await _userRepository.ValidateCredentialsAsync(username, pin);

        if (user == null)
            return (false, "Invalid username or PIN");

        _currentUser = user;
        UserChanged?.Invoke(this, user);
        return (true, $"Welcome, {user.DisplayName}!");
    }

    public void Logout()
    {
        _currentUser = null;
        UserChanged?.Invoke(this, null);
    }

    public async Task<bool> ValidateManagerPinAsync(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
            return false;

        return await _userRepository.ValidateManagerPinAsync(pin);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }
}
