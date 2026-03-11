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
}
