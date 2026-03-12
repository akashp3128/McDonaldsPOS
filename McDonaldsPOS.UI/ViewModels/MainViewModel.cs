using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using McDonaldsPOS.Services;

namespace McDonaldsPOS.UI.ViewModels;

/// <summary>
/// Main window ViewModel - handles navigation between views
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private bool _isOnPOS = true;

    [ObservableProperty]
    private bool _isOnAnalytics;

    // ViewModels
    public LoginViewModel LoginViewModel { get; }
    public POSViewModel POSViewModel { get; }
    public AnalyticsViewModel AnalyticsViewModel { get; }

    public MainViewModel(
        IAuthService authService,
        LoginViewModel loginViewModel,
        POSViewModel posViewModel,
        AnalyticsViewModel analyticsViewModel)
    {
        _authService = authService;
        LoginViewModel = loginViewModel;
        POSViewModel = posViewModel;
        AnalyticsViewModel = analyticsViewModel;

        // Subscribe to auth changes
        _authService.UserChanged += OnUserChanged;

        // Subscribe to login success
        LoginViewModel.LoginSuccessful += OnLoginSuccessful;

        // Start with login view
        CurrentViewModel = LoginViewModel;
    }

    private void OnUserChanged(object? sender, Core.Models.User? user)
    {
        IsLoggedIn = user != null;
        CurrentUserName = user?.DisplayName ?? string.Empty;
    }

    private void OnLoginSuccessful(object? sender, EventArgs e)
    {
        NavigateToPOS();
    }

    [RelayCommand]
    private void NavigateToPOS()
    {
        CurrentViewModel = POSViewModel;
        POSViewModel.InitializeAsync();
        IsOnPOS = true;
        IsOnAnalytics = false;
    }

    [RelayCommand]
    private void NavigateToAnalytics()
    {
        CurrentViewModel = AnalyticsViewModel;
        AnalyticsViewModel.InitializeAsync();
        IsOnPOS = false;
        IsOnAnalytics = true;
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        LoginViewModel.Reset();
        CurrentViewModel = LoginViewModel;
        IsOnPOS = true;
        IsOnAnalytics = false;
    }
}
