using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Services;

namespace McDonaldsPOS.UI.ViewModels;

/// <summary>
/// ViewModel for the login screen
/// </summary>
public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _selectedUsername = string.Empty;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Enter your credentials to sign in";

    [ObservableProperty]
    private bool _isStatusError;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private User? _selectedUser;

    public event EventHandler? LoginSuccessful;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        _ = LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        var users = await _authService.GetActiveUsersAsync();
        Users = new ObservableCollection<User>(users);
    }

    partial void OnSelectedUserChanged(User? value)
    {
        if (value != null)
        {
            SelectedUsername = value.Username;
        }
    }

    [RelayCommand]
    private void AppendPin(string digit)
    {
        if (Pin.Length < 4)
        {
            Pin += digit;
        }

        // Auto-login when 4 digits entered
        if (Pin.Length == 4 && !string.IsNullOrEmpty(SelectedUsername))
        {
            _ = LoginAsync();
        }
    }

    [RelayCommand]
    private void ClearPin()
    {
        Pin = string.Empty;
        ClearError();
        StatusMessage = "Enter your credentials to sign in";
        IsStatusError = false;
    }

    [RelayCommand]
    private void BackspacePin()
    {
        if (Pin.Length > 0)
        {
            Pin = Pin[..^1];
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(SelectedUsername))
        {
            StatusMessage = "Please select a user first";
            IsStatusError = true;
            return;
        }

        if (Pin.Length != 4)
        {
            StatusMessage = "PIN must be 4 digits";
            IsStatusError = true;
            return;
        }

        IsLoading = true;
        ClearError();

        var (success, message) = await _authService.LoginAsync(SelectedUsername, Pin);

        IsLoading = false;

        if (success)
        {
            StatusMessage = message;
            IsStatusError = false;
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            StatusMessage = message;
            IsStatusError = true;
            Pin = string.Empty;
        }
    }

    public void Reset()
    {
        SelectedUsername = string.Empty;
        SelectedUser = null;
        Pin = string.Empty;
        StatusMessage = "Enter your credentials to sign in";
        IsStatusError = false;
        ClearError();
    }
}
