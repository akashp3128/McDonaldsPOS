using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Data.Repositories;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service for order management
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuService _menuService;
    private readonly IAuthService _authService;

    private Order? _currentOrder;

    public Order CurrentOrder => _currentOrder ??= CreateOrderInMemory();
    public bool HasActiveOrder => _currentOrder != null && _currentOrder.Items.Any();

    public event EventHandler<Order>? OrderUpdated;
    public event EventHandler<Order>? OrderSentToKitchen;

    public OrderService(
        IOrderRepository orderRepository,
        IMenuService menuService,
        IAuthService authService)
    {
        _orderRepository = orderRepository;
        _menuService = menuService;
        _authService = authService;
    }

    private Order CreateOrderInMemory()
    {
        return new Order
        {
            Status = OrderStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _authService.CurrentUser?.Id
        };
    }

    public async Task<Order> CreateNewOrderAsync(int? userId = null)
    {
        _currentOrder = new Order
        {
            Status = OrderStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId ?? _authService.CurrentUser?.Id
        };

        NotifyOrderUpdated();
        return await Task.FromResult(_currentOrder);
    }

    public async Task<OrderItem> AddItemAsync(MenuItem menuItem, ItemSize size = ItemSize.None, int quantity = 1)
    {
        var order = CurrentOrder;

        // Determine price based on size
        var unitPrice = _menuService.GetPriceForSize(menuItem, size);

        var orderItem = new OrderItem
        {
            MenuItemId = menuItem.Id,
            MenuItem = menuItem,
            ItemName = menuItem.Name,
            Size = menuItem.IsSizable ? (size == ItemSize.None ? ItemSize.Medium : size) : ItemSize.None,
            UnitPrice = unitPrice,
            Quantity = quantity
        };

        order.Items.Add(orderItem);
        order.RecalculateTotals();
        NotifyOrderUpdated();

        return await Task.FromResult(orderItem);
    }

    public async Task<OrderItem> AddComboAsync(MenuItem mainItem, MenuItem side, MenuItem drink, ItemSize size = ItemSize.Medium)
    {
        var order = CurrentOrder;

        // Add main item
        var mainOrderItem = new OrderItem
        {
            MenuItemId = mainItem.Id,
            MenuItem = mainItem,
            ItemName = $"{mainItem.Name} Meal",
            Size = ItemSize.None,
            UnitPrice = mainItem.BasePrice,
            Quantity = 1,
            IsCombo = true
        };
        order.Items.Add(mainOrderItem);

        // Add side (linked to main)
        var sidePrice = _menuService.GetPriceForSize(side, size);
        var sideOrderItem = new OrderItem
        {
            MenuItemId = side.Id,
            MenuItem = side,
            ItemName = side.Name,
            Size = size,
            UnitPrice = sidePrice,
            Quantity = 1,
            IsCombo = true,
            ParentComboItemId = mainOrderItem.Id
        };
        order.Items.Add(sideOrderItem);

        // Add drink (linked to main)
        var drinkPrice = _menuService.GetPriceForSize(drink, size);
        var drinkOrderItem = new OrderItem
        {
            MenuItemId = drink.Id,
            MenuItem = drink,
            ItemName = drink.Name,
            Size = size,
            UnitPrice = drinkPrice,
            Quantity = 1,
            IsCombo = true,
            ParentComboItemId = mainOrderItem.Id
        };
        order.Items.Add(drinkOrderItem);

        order.RecalculateTotals();
        NotifyOrderUpdated();

        return await Task.FromResult(mainOrderItem);
    }

    public async Task RemoveItemAsync(OrderItem item)
    {
        var order = CurrentOrder;

        // If it's a combo main item, remove linked items too
        if (item.IsCombo && item.ParentComboItemId == null)
        {
            var linkedItems = order.Items.Where(i => i.ParentComboItemId == item.Id).ToList();
            foreach (var linked in linkedItems)
            {
                order.Items.Remove(linked);
            }
        }

        order.Items.Remove(item);
        order.RecalculateTotals();
        NotifyOrderUpdated();

        await Task.CompletedTask;
    }

    public async Task UpdateItemQuantityAsync(OrderItem item, int newQuantity)
    {
        if (newQuantity <= 0)
        {
            await RemoveItemAsync(item);
            return;
        }

        item.Quantity = newQuantity;
        CurrentOrder.RecalculateTotals();
        NotifyOrderUpdated();

        await Task.CompletedTask;
    }

    public async Task AddModifierToItemAsync(OrderItem item, Modifier modifier)
    {
        // Check if modifier already exists
        if (item.Modifiers.Any(m => m.ModifierId == modifier.Id))
            return;

        var orderModifier = new OrderItemModifier
        {
            ModifierId = modifier.Id,
            Modifier = modifier,
            ModifierName = modifier.ShortName,
            PriceAdjustment = modifier.PriceAdjustment
        };

        item.Modifiers.Add(orderModifier);
        CurrentOrder.RecalculateTotals();
        NotifyOrderUpdated();

        await Task.CompletedTask;
    }

    public async Task RemoveModifierFromItemAsync(OrderItem item, OrderItemModifier modifier)
    {
        item.Modifiers.Remove(modifier);
        CurrentOrder.RecalculateTotals();
        NotifyOrderUpdated();

        await Task.CompletedTask;
    }

    public async Task<bool> VoidItemAsync(OrderItem item, string reason, string managerPin)
    {
        // Validate manager PIN
        var isValid = await _authService.ValidateManagerPinAsync(managerPin);
        if (!isValid)
            return false;

        item.IsVoided = true;
        item.VoidReason = reason;

        // Find manager user by PIN (for audit)
        // In production, we'd get the actual user

        CurrentOrder.RecalculateTotals();
        NotifyOrderUpdated();

        return true;
    }

    public async Task<Order> SendToKitchenAsync()
    {
        var order = CurrentOrder;

        if (!order.Items.Any(i => !i.IsVoided))
            throw new InvalidOperationException("Cannot send empty order to kitchen");

        // Get next order number
        order.OrderNumber = await _orderRepository.GetNextOrderNumberAsync();
        order.Status = OrderStatus.Pending;
        order.RecalculateTotals();

        // Save to database
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        OrderSentToKitchen?.Invoke(this, order);

        // Start a new order
        _currentOrder = null;
        NotifyOrderUpdated();

        return order;
    }

    public async Task<Order> CompletePaymentAsync(PaymentMethod method, decimal amountTendered)
    {
        var order = CurrentOrder;

        if (!order.Items.Any(i => !i.IsVoided))
            throw new InvalidOperationException("Cannot complete payment on empty order");

        order.PaymentMethod = method;
        order.AmountTendered = amountTendered;
        order.ChangeGiven = method == PaymentMethod.Cash
            ? Math.Max(0, amountTendered - order.Total)
            : 0;
        order.PaidAt = DateTime.UtcNow;

        // If not already in kitchen, send it
        bool sendingToKitchen = order.Status == OrderStatus.Draft;
        if (sendingToKitchen)
        {
            order.OrderNumber = await _orderRepository.GetNextOrderNumberAsync();
            order.Status = OrderStatus.Pending;
        }

        order.RecalculateTotals();

        // Save to database
        if (order.Id == 0)
        {
            await _orderRepository.AddAsync(order);
        }
        else
        {
            await _orderRepository.UpdateAsync(order);
        }
        await _orderRepository.SaveChangesAsync();

        var completedOrder = order;

        // Notify KDS if order was just sent to kitchen
        if (sendingToKitchen)
        {
            OrderSentToKitchen?.Invoke(this, completedOrder);
        }

        // Start a new order
        _currentOrder = null;
        NotifyOrderUpdated();

        return completedOrder;
    }

    public void ClearCurrentOrder()
    {
        _currentOrder = null;
        NotifyOrderUpdated();
    }

    public async Task<IEnumerable<Order>> GetKitchenOrdersAsync()
    {
        return await _orderRepository.GetActiveKitchenOrdersAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetWithItemsAsync(orderId);
        if (order == null) return;

        order.Status = newStatus;

        if (newStatus == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();
    }

    public async Task BumpOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetWithItemsAsync(orderId);
        if (order == null) return;

        // Progress the order status
        order.Status = order.Status switch
        {
            OrderStatus.Pending => OrderStatus.InPrep,
            OrderStatus.InPrep => OrderStatus.Ready,
            OrderStatus.Ready => OrderStatus.Completed,
            _ => order.Status
        };

        if (order.Status == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetTodaysOrdersAsync()
    {
        return await _orderRepository.GetTodaysOrdersAsync();
    }

    private void NotifyOrderUpdated()
    {
        OrderUpdated?.Invoke(this, CurrentOrder);
    }
}
