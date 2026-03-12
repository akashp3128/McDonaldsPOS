using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Services;

namespace McDonaldsPOS.UI.ViewModels;

/// <summary>
/// Kitchen Display System (KDS) ViewModel
/// </summary>
public partial class KDSViewModel : ViewModelBase
{
    private readonly IOrderService _orderService;
    private readonly DispatcherTimer _refreshTimer;

    [ObservableProperty]
    private ObservableCollection<KDSOrderViewModel> _pendingOrders = new();

    [ObservableProperty]
    private ObservableCollection<KDSOrderViewModel> _inPrepOrders = new();

    [ObservableProperty]
    private ObservableCollection<KDSOrderViewModel> _readyOrders = new();

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private int _inPrepCount;

    [ObservableProperty]
    private int _readyCount;

    public KDSViewModel(IOrderService orderService)
    {
        _orderService = orderService;

        // Subscribe to new orders
        _orderService.OrderSentToKitchen += OnOrderSentToKitchen;

        // Auto-refresh timer
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _refreshTimer.Tick += async (s, e) => await RefreshOrdersAsync();
    }

    public void StartRefresh()
    {
        _refreshTimer.Start();
        _ = RefreshOrdersAsync();
    }

    public void StopRefresh()
    {
        _refreshTimer.Stop();
    }

    private void OnOrderSentToKitchen(object? sender, Order order)
    {
        _ = RefreshOrdersAsync();
    }

    [RelayCommand]
    private async Task RefreshOrdersAsync()
    {
        var orders = await _orderService.GetKitchenOrdersAsync();

        var orderVMs = orders.Select(o => new KDSOrderViewModel(o)).ToList();

        PendingOrders = new ObservableCollection<KDSOrderViewModel>(
            orderVMs.Where(o => o.Order.Status == OrderStatus.Pending));

        InPrepOrders = new ObservableCollection<KDSOrderViewModel>(
            orderVMs.Where(o => o.Order.Status == OrderStatus.InPrep));

        ReadyOrders = new ObservableCollection<KDSOrderViewModel>(
            orderVMs.Where(o => o.Order.Status == OrderStatus.Ready));

        PendingCount = PendingOrders.Count;
        InPrepCount = InPrepOrders.Count;
        ReadyCount = ReadyOrders.Count;
    }

    [RelayCommand]
    private async Task BumpOrder(KDSOrderViewModel orderVM)
    {
        await _orderService.BumpOrderAsync(orderVM.Order.Id);
        await RefreshOrdersAsync();
    }

    [RelayCommand]
    private async Task StartOrder(KDSOrderViewModel orderVM)
    {
        await _orderService.UpdateOrderStatusAsync(orderVM.Order.Id, OrderStatus.InPrep);
        await RefreshOrdersAsync();
    }

    [RelayCommand]
    private async Task CompleteOrder(KDSOrderViewModel orderVM)
    {
        await _orderService.UpdateOrderStatusAsync(orderVM.Order.Id, OrderStatus.Completed);
        await RefreshOrdersAsync();
    }
}

public partial class KDSOrderViewModel : ObservableObject
{
    public Order Order { get; }

    public int OrderNumber => Order.OrderNumber;
    public string OrderTime => Order.CreatedAt.ToLocalTime().ToString("h:mm tt");
    public string StatusText => Order.Status.ToString();
    public string StatusColor => Order.Status switch
    {
        OrderStatus.Pending => "#FFC107",  // Yellow
        OrderStatus.InPrep => "#2196F3",   // Blue
        OrderStatus.Ready => "#4CAF50",    // Green
        _ => "#757575"
    };

    public TimeSpan ElapsedTime => DateTime.UtcNow - Order.CreatedAt;
    public string ElapsedDisplay => ElapsedTime.TotalMinutes < 1
        ? "< 1 min"
        : $"{(int)ElapsedTime.TotalMinutes} min";

    public bool IsOverdue => ElapsedTime.TotalMinutes > 5;

    public ObservableCollection<KDSItemViewModel> Items { get; }

    public KDSOrderViewModel(Order order)
    {
        Order = order;
        Items = new ObservableCollection<KDSItemViewModel>(
            order.Items
                .Where(i => !i.IsVoided)
                .Select(i => new KDSItemViewModel(i)));
    }
}

public class KDSItemViewModel
{
    private readonly OrderItem _item;

    public string Name => _item.DisplayName;
    public int Quantity => _item.Quantity;
    public string Modifiers => _item.ModifierSummary;
    public bool HasModifiers => !string.IsNullOrEmpty(Modifiers);
    public bool IsCombo => _item.IsCombo;

    public KDSItemViewModel(OrderItem item)
    {
        _item = item;
    }
}
