using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using McDonaldsPOS.Data.Repositories;

namespace McDonaldsPOS.UI.ViewModels;

public partial class AnalyticsViewModel : ViewModelBase
{
    private readonly IOrderRepository _orderRepository;

    // Summary Cards
    [ObservableProperty]
    private decimal _totalSales;

    [ObservableProperty]
    private int _orderCount;

    [ObservableProperty]
    private decimal _averageOrderValue;

    [ObservableProperty]
    private int _itemsSold;

    [ObservableProperty]
    private decimal _cashSales;

    [ObservableProperty]
    private decimal _cardSales;

    // Charts Data
    [ObservableProperty]
    private ObservableCollection<HourlySalesViewModel> _hourlySales = new();

    [ObservableProperty]
    private ObservableCollection<TopItemViewModel> _topSellingItems = new();

    [ObservableProperty]
    private ObservableCollection<CategorySalesViewModel> _categorySales = new();

    // Date Selection
    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private string _dateDisplay = "Today";

    // Peak Hour
    [ObservableProperty]
    private string _peakHour = "--";

    [ObservableProperty]
    private decimal _peakHourSales;

    public AnalyticsViewModel(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async void InitializeAsync()
    {
        await LoadAnalyticsAsync();
    }

    [RelayCommand]
    private async Task LoadAnalyticsAsync()
    {
        IsLoading = true;

        try
        {
            // Load summary
            var analytics = await _orderRepository.GetSalesAnalyticsAsync(SelectedDate);
            TotalSales = analytics.TotalSales;
            OrderCount = analytics.OrderCount;
            AverageOrderValue = analytics.AverageOrderValue;
            ItemsSold = analytics.ItemsSold;
            CashSales = analytics.CashSales;
            CardSales = analytics.CardSales;

            // Load hourly sales
            var hourly = await _orderRepository.GetHourlySalesAsync(SelectedDate);
            var hourlyList = hourly.ToList();

            // Find max for scaling
            var maxSales = hourlyList.Max(h => h.Sales);
            if (maxSales == 0) maxSales = 1; // Avoid division by zero

            HourlySales = new ObservableCollection<HourlySalesViewModel>(
                hourlyList.Select(h => new HourlySalesViewModel(h.Hour, h.Sales, h.OrderCount, maxSales)));

            // Find peak hour
            var peak = hourlyList.OrderByDescending(h => h.Sales).FirstOrDefault();
            if (peak != null && peak.Sales > 0)
            {
                PeakHour = FormatHour(peak.Hour);
                PeakHourSales = peak.Sales;
            }
            else
            {
                PeakHour = "--";
                PeakHourSales = 0;
            }

            // Load top selling items
            var topItems = await _orderRepository.GetTopSellingItemsAsync(SelectedDate, 5);
            var topList = topItems.ToList();
            var maxQty = topList.FirstOrDefault()?.QuantitySold ?? 1;

            TopSellingItems = new ObservableCollection<TopItemViewModel>(
                topList.Select(t => new TopItemViewModel(t.ItemName, t.QuantitySold, t.Revenue, maxQty)));

            // Load category sales
            var categories = await _orderRepository.GetSalesByCategoryAsync(SelectedDate);
            var categoryList = categories.ToList();
            var totalCategorySales = categoryList.Sum(c => c.Sales);
            if (totalCategorySales == 0) totalCategorySales = 1;

            CategorySales = new ObservableCollection<CategorySalesViewModel>(
                categoryList.Select(c => new CategorySalesViewModel(c.Category, c.Sales, c.ItemCount, totalCategorySales)));

            UpdateDateDisplay();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task PreviousDay()
    {
        SelectedDate = SelectedDate.AddDays(-1);
        await LoadAnalyticsAsync();
    }

    [RelayCommand]
    private async Task NextDay()
    {
        if (SelectedDate.Date < DateTime.Today)
        {
            SelectedDate = SelectedDate.AddDays(1);
            await LoadAnalyticsAsync();
        }
    }

    [RelayCommand]
    private async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        await LoadAnalyticsAsync();
    }

    private void UpdateDateDisplay()
    {
        if (SelectedDate.Date == DateTime.Today)
            DateDisplay = "Today";
        else if (SelectedDate.Date == DateTime.Today.AddDays(-1))
            DateDisplay = "Yesterday";
        else
            DateDisplay = SelectedDate.ToString("MMM d, yyyy");
    }

    private static string FormatHour(int hour)
    {
        return hour switch
        {
            0 => "12 AM",
            12 => "12 PM",
            < 12 => $"{hour} AM",
            _ => $"{hour - 12} PM"
        };
    }
}

public class HourlySalesViewModel
{
    public int Hour { get; }
    public string HourLabel { get; }
    public decimal Sales { get; }
    public int OrderCount { get; }
    public double BarHeight { get; }
    public string Tooltip { get; }

    public HourlySalesViewModel(int hour, decimal sales, int orderCount, decimal maxSales)
    {
        Hour = hour;
        HourLabel = hour switch
        {
            0 => "12a",
            12 => "12p",
            < 12 => $"{hour}a",
            _ => $"{hour - 12}p"
        };
        Sales = sales;
        OrderCount = orderCount;
        BarHeight = maxSales > 0 ? (double)(sales / maxSales) * 150 : 0; // Max height 150px
        Tooltip = $"{HourLabel}: ${sales:F2} ({orderCount} orders)";
    }
}

public class TopItemViewModel
{
    public string ItemName { get; }
    public int QuantitySold { get; }
    public decimal Revenue { get; }
    public double BarWidth { get; }
    public string RevenueDisplay { get; }

    public TopItemViewModel(string itemName, int quantitySold, decimal revenue, int maxQty)
    {
        ItemName = itemName;
        QuantitySold = quantitySold;
        Revenue = revenue;
        BarWidth = maxQty > 0 ? (double)quantitySold / maxQty * 200 : 0; // Max width 200px
        RevenueDisplay = $"${revenue:F2}";
    }
}

public class CategorySalesViewModel
{
    public string Category { get; }
    public decimal Sales { get; }
    public int ItemCount { get; }
    public double Percentage { get; }
    public string PercentageDisplay { get; }
    public string SalesDisplay { get; }
    public string Color { get; }

    private static readonly Dictionary<string, string> CategoryColors = new()
    {
        { "Burgers", "#D32F2F" },
        { "Chicken", "#FF9800" },
        { "FriesSides", "#FFC107" },
        { "Drinks", "#2196F3" },
        { "Desserts", "#9C27B0" },
        { "Breakfast", "#FFEB3B" },
        { "HappyMeals", "#E91E63" },
        { "McCafe", "#795548" }
    };

    public CategorySalesViewModel(string category, decimal sales, int itemCount, decimal totalSales)
    {
        Category = category;
        Sales = sales;
        ItemCount = itemCount;
        Percentage = totalSales > 0 ? (double)(sales / totalSales) * 100 : 0;
        PercentageDisplay = $"{Percentage:F1}%";
        SalesDisplay = $"${sales:F2}";
        Color = CategoryColors.GetValueOrDefault(category, "#757575");
    }
}
