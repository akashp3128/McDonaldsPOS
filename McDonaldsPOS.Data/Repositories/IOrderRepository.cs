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
}
