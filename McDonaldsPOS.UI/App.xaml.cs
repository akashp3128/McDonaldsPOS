using System.Windows;
using McDonaldsPOS.Data;
using McDonaldsPOS.Data.Repositories;
using McDonaldsPOS.Services;
using McDonaldsPOS.UI.ViewModels;
using McDonaldsPOS.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace McDonaldsPOS.UI;

/// <summary>
/// Application entry point with DI configuration
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public static ServiceProvider Services { get; private set; } = null!;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database - use a single DbContext instance for the app
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=mcdonalds_pos.db"),
            ServiceLifetime.Singleton);

        // Repositories - singleton to share with singleton services
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IMenuRepository, MenuRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();

        // Services - singleton for state management
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IMenuService, MenuService>();
        services.AddSingleton<IOrderService, OrderService>();

        // ViewModels - singleton for state persistence
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<POSViewModel>();
        services.AddSingleton<KDSViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<KDSWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize and seed database
        var context = _serviceProvider.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context);

        // Show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }

    /// <summary>
    /// Opens the KDS window
    /// </summary>
    public void OpenKDSWindow()
    {
        var kdsWindow = new KDSWindow
        {
            DataContext = _serviceProvider.GetRequiredService<KDSViewModel>()
        };
        kdsWindow.Show();
    }
}
