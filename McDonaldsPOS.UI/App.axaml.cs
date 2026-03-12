using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using McDonaldsPOS.Data;
using McDonaldsPOS.Data.Repositories;
using McDonaldsPOS.Services;
using McDonaldsPOS.UI.ViewModels;
using McDonaldsPOS.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace McDonaldsPOS.UI;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    public static ServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        // Initialize database
        var context = _serviceProvider.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=mcdonalds_pos.db"),
            ServiceLifetime.Singleton);

        // Repositories
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IMenuRepository, MenuRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();

        // Services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IMenuService, MenuService>();
        services.AddSingleton<IOrderService, OrderService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<POSViewModel>();
        services.AddSingleton<KDSViewModel>();
        services.AddSingleton<AnalyticsViewModel>();
    }

    public void OpenKDSWindow()
    {
        if (_serviceProvider == null) return;

        var kdsWindow = new KDSWindow
        {
            DataContext = _serviceProvider.GetRequiredService<KDSViewModel>()
        };
        kdsWindow.Show();
    }
}
