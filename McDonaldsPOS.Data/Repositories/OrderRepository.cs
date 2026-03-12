using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository for order-related data operations
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetWithItemsAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Items)
            .ThenInclude(i => i.Modifiers)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .Include(o => o.CreatedByUser)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Where(o => o.Status == status)
            .Include(o => o.Items)
            .ThenInclude(i => i.Modifiers)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetActiveKitchenOrdersAsync()
    {
        return await _dbSet
            .Where(o => o.Status == OrderStatus.Pending ||
                       o.Status == OrderStatus.InPrep ||
                       o.Status == OrderStatus.Ready)
            .Include(o => o.Items.Where(i => !i.IsVoided))
            .ThenInclude(i => i.Modifiers)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetNextOrderNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var maxOrderNumber = await _dbSet
            .Where(o => o.CreatedAt >= today)
            .MaxAsync(o => (int?)o.OrderNumber) ?? 0;
        return maxOrderNumber + 1;
    }

    public async Task<IEnumerable<Order>> GetTodaysOrdersAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(o => o.CreatedAt >= today)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<SalesAnalytics> GetSalesAnalyticsAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var orders = await _dbSet
            .Where(o => o.CreatedAt >= startOfDay && o.CreatedAt < endOfDay && o.PaidAt != null)
            .Include(o => o.Items.Where(i => !i.IsVoided))
            .ToListAsync();

        var totalSales = orders.Sum(o => o.Total);
        var orderCount = orders.Count;
        var avgOrderValue = orderCount > 0 ? totalSales / orderCount : 0;
        var itemsSold = orders.SelectMany(o => o.Items).Sum(i => i.Quantity);
        var cashSales = orders.Where(o => o.PaymentMethod == PaymentMethod.Cash).Sum(o => o.Total);
        var cardSales = orders.Where(o => o.PaymentMethod == PaymentMethod.Card).Sum(o => o.Total);

        return new SalesAnalytics(totalSales, orderCount, avgOrderValue, itemsSold, cashSales, cardSales);
    }

    public async Task<IEnumerable<HourlySales>> GetHourlySalesAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var orders = await _dbSet
            .Where(o => o.CreatedAt >= startOfDay && o.CreatedAt < endOfDay && o.PaidAt != null)
            .ToListAsync();

        var hourlySales = orders
            .GroupBy(o => o.CreatedAt.ToLocalTime().Hour)
            .Select(g => new HourlySales(g.Key, g.Sum(o => o.Total), g.Count()))
            .OrderBy(h => h.Hour)
            .ToList();

        // Fill in missing hours with zeros
        var allHours = Enumerable.Range(0, 24)
            .Select(h => hourlySales.FirstOrDefault(hs => hs.Hour == h) ?? new HourlySales(h, 0, 0))
            .ToList();

        return allHours;
    }

    public async Task<IEnumerable<TopSellingItem>> GetTopSellingItemsAsync(DateTime date, int count = 5)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var orders = await _dbSet
            .Where(o => o.CreatedAt >= startOfDay && o.CreatedAt < endOfDay && o.PaidAt != null)
            .Include(o => o.Items.Where(i => !i.IsVoided))
            .ToListAsync();

        var topItems = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ItemName)
            .Select(g => new TopSellingItem(
                g.Key,
                g.Sum(i => i.Quantity),
                g.Sum(i => i.LineTotal)))
            .OrderByDescending(t => t.QuantitySold)
            .Take(count)
            .ToList();

        return topItems;
    }

    public async Task<IEnumerable<CategorySales>> GetSalesByCategoryAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var orders = await _dbSet
            .Where(o => o.CreatedAt >= startOfDay && o.CreatedAt < endOfDay && o.PaidAt != null)
            .Include(o => o.Items.Where(i => !i.IsVoided))
            .ThenInclude(i => i.MenuItem)
            .ToListAsync();

        var categorySales = orders
            .SelectMany(o => o.Items)
            .Where(i => i.MenuItem != null)
            .GroupBy(i => i.MenuItem!.Category.ToString())
            .Select(g => new CategorySales(
                g.Key,
                g.Sum(i => i.LineTotal),
                g.Sum(i => i.Quantity)))
            .OrderByDescending(c => c.Sales)
            .ToList();

        return categorySales;
    }
}
