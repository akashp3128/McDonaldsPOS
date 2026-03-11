using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service interface for order management
/// </summary>
public interface IOrderService
{
    Order CurrentOrder { get; }
    bool HasActiveOrder { get; }

    // Order lifecycle
    Task<Order> CreateNewOrderAsync(int? userId = null);
    Task<Order> SendToKitchenAsync();
    Task<Order> CompletePaymentAsync(PaymentMethod method, decimal amountTendered);
    void ClearCurrentOrder();

    // Order item management
    Task<OrderItem> AddItemAsync(MenuItem menuItem, ItemSize size = ItemSize.None, int quantity = 1);
    Task<OrderItem> AddComboAsync(MenuItem mainItem, MenuItem side, MenuItem drink, ItemSize size = ItemSize.Medium);
    Task RemoveItemAsync(OrderItem item);
    Task UpdateItemQuantityAsync(OrderItem item, int newQuantity);
    Task AddModifierToItemAsync(OrderItem item, Modifier modifier);
    Task RemoveModifierFromItemAsync(OrderItem item, OrderItemModifier modifier);
    Task<bool> VoidItemAsync(OrderItem item, string reason, string managerPin);

    // Kitchen operations
    Task<IEnumerable<Order>> GetKitchenOrdersAsync();
    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    Task BumpOrderAsync(int orderId);

    // Reporting
    Task<IEnumerable<Order>> GetTodaysOrdersAsync();

    event EventHandler<Order>? OrderUpdated;
    event EventHandler<Order>? OrderSentToKitchen;
}
