using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository interface for order-related operations
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(int id);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetActiveKitchenOrdersAsync();
    Task<int> GetNextOrderNumberAsync();
    Task<IEnumerable<Order>> GetTodaysOrdersAsync();

    // Analytics
    Task<SalesAnalytics> GetSalesAnalyticsAsync(DateTime date);
    Task<IEnumerable<HourlySales>> GetHourlySalesAsync(DateTime date);
    Task<IEnumerable<TopSellingItem>> GetTopSellingItemsAsync(DateTime date, int count = 5);
    Task<IEnumerable<CategorySales>> GetSalesByCategoryAsync(DateTime date);
}

// Analytics DTOs
public record SalesAnalytics(
    decimal TotalSales,
    int OrderCount,
    decimal AverageOrderValue,
    int ItemsSold,
    decimal CashSales,
    decimal CardSales
);

public record HourlySales(int Hour, decimal Sales, int OrderCount);

public record TopSellingItem(string ItemName, int QuantitySold, decimal Revenue);

public record CategorySales(string Category, decimal Sales, int ItemCount);
