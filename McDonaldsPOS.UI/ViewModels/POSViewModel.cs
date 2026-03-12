using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Services;

namespace McDonaldsPOS.UI.ViewModels;

/// <summary>
/// Main POS screen ViewModel
/// </summary>
public partial class POSViewModel : ViewModelBase
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
    private readonly IAuthService _authService;

    // Categories
    [ObservableProperty]
    private ObservableCollection<CategoryViewModel> _categories = new();

    [ObservableProperty]
    private CategoryViewModel? _selectedCategory;

    // Menu Items for selected category
    [ObservableProperty]
    private ObservableCollection<MenuItemViewModel> _menuItems = new();

    // Current Order
    [ObservableProperty]
    private ObservableCollection<OrderItemViewModel> _orderItems = new();

    [ObservableProperty]
    private OrderItemViewModel? _selectedOrderItem;

    [ObservableProperty]
    private decimal _subtotal;

    [ObservableProperty]
    private decimal _tax;

    [ObservableProperty]
    private decimal _total;

    [ObservableProperty]
    private int _itemCount;

    // Dialogs
    [ObservableProperty]
    private bool _showModifierDialog;

    [ObservableProperty]
    private bool _showComboPrompt;

    [ObservableProperty]
    private bool _showPaymentDialog;

    [ObservableProperty]
    private bool _showVoidDialog;

    [ObservableProperty]
    private bool _showReceiptDialog;

    [ObservableProperty]
    private bool _showSizeDialog;

    // Modifier dialog state
    [ObservableProperty]
    private ObservableCollection<ModifierViewModel> _availableModifiers = new();

    [ObservableProperty]
    private OrderItemViewModel? _modifierTargetItem;

    // Combo prompt state
    [ObservableProperty]
    private MenuItem? _comboMainItem;

    // Payment state
    [ObservableProperty]
    private string _tenderedAmount = string.Empty;

    [ObservableProperty]
    private string _tenderedAmountDisplay = "$0.00";

    [ObservableProperty]
    private decimal _changeAmount;

    [ObservableProperty]
    private bool _isCardPayment;

    [ObservableProperty]
    private string _paymentError = string.Empty;

    [ObservableProperty]
    private bool _hasPaymentError;

    // Void state
    [ObservableProperty]
    private string _voidPin = string.Empty;

    [ObservableProperty]
    private string _voidReason = string.Empty;

    // Receipt state
    [ObservableProperty]
    private Order? _completedOrder;

    // Size dialog state
    [ObservableProperty]
    private MenuItem? _sizeTargetItem;

    public POSViewModel(
        IMenuService menuService,
        IOrderService orderService,
        IAuthService authService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _authService = authService;

        _orderService.OrderUpdated += OnOrderUpdated;

        InitializeCategories();
    }

    private void InitializeCategories()
    {
        Categories = new ObservableCollection<CategoryViewModel>
        {
            new(MenuCategory.Burgers, "Burgers", "#D32F2F"),
            new(MenuCategory.Chicken, "Chicken", "#FF9800"),
            new(MenuCategory.FriesSides, "Fries & Sides", "#FFC107"),
            new(MenuCategory.Drinks, "Drinks", "#2196F3"),
            new(MenuCategory.Desserts, "Desserts", "#9C27B0"),
            new(MenuCategory.Breakfast, "Breakfast", "#FFEB3B"),
            new(MenuCategory.HappyMeals, "Happy Meals", "#E91E63"),
            new(MenuCategory.McCafe, "McCafe", "#795548")
        };
    }

    public async void InitializeAsync()
    {
        if (Categories.Any() && SelectedCategory == null)
        {
            SelectedCategory = Categories.First();
        }
        await LoadMenuItemsAsync();
        RefreshOrderDisplay();
    }

    partial void OnSelectedCategoryChanged(CategoryViewModel? value)
    {
        if (value != null)
        {
            _ = LoadMenuItemsAsync();
        }
    }

    [RelayCommand]
    private void SelectCategory(CategoryViewModel category)
    {
        SelectedCategory = category;
    }

    private async Task LoadMenuItemsAsync()
    {
        if (SelectedCategory == null) return;

        IsLoading = true;
        var items = await _menuService.GetMenuItemsByCategoryAsync(SelectedCategory.Category);

        MenuItems = new ObservableCollection<MenuItemViewModel>(
            items.Select(i => new MenuItemViewModel(i)));

        IsLoading = false;
    }

    [RelayCommand]
    private async Task SelectMenuItem(MenuItemViewModel item)
    {
        // If item is sizable, show size dialog first
        if (item.MenuItem.IsSizable)
        {
            SizeTargetItem = item.MenuItem;
            ShowSizeDialog = true;
            return;
        }

        // Add item to order
        await AddItemToOrderAsync(item.MenuItem, ItemSize.None);
    }

    [RelayCommand]
    private async Task SelectSize(string sizeStr)
    {
        if (SizeTargetItem == null) return;

        var size = sizeStr switch
        {
            "Small" => ItemSize.Small,
            "Medium" => ItemSize.Medium,
            "Large" => ItemSize.Large,
            _ => ItemSize.Medium
        };

        ShowSizeDialog = false;
        await AddItemToOrderAsync(SizeTargetItem, size);
        SizeTargetItem = null;
    }

    [RelayCommand]
    private void CancelSizeDialog()
    {
        ShowSizeDialog = false;
        SizeTargetItem = null;
    }

    private async Task AddItemToOrderAsync(MenuItem menuItem, ItemSize size)
    {
        var orderItem = await _orderService.AddItemAsync(menuItem, size);

        // Check if item is combo-eligible and prompt for meal
        if (menuItem.IsComboEligible)
        {
            ComboMainItem = menuItem;
            ShowComboPrompt = true;
        }

        RefreshOrderDisplay();
    }

    [RelayCommand]
    private async Task MakeItAMeal()
    {
        if (ComboMainItem == null) return;

        ShowComboPrompt = false;

        // Get default combo items
        var (side, drink) = await _menuService.GetDefaultComboItemsAsync(ComboMainItem);

        if (side != null && drink != null)
        {
            // Remove the single item we just added
            var lastItem = OrderItems.LastOrDefault();
            if (lastItem != null)
            {
                await _orderService.RemoveItemAsync(lastItem.OrderItem);
            }

            // Add as combo
            await _orderService.AddComboAsync(ComboMainItem, side, drink);
            RefreshOrderDisplay();
        }

        ComboMainItem = null;
    }

    [RelayCommand]
    private void NoMeal()
    {
        ShowComboPrompt = false;
        ComboMainItem = null;
    }

    [RelayCommand]
    private void SelectOrderItem(OrderItemViewModel item)
    {
        SelectedOrderItem = item;
    }

    [RelayCommand]
    private async Task IncreaseQuantity(OrderItemViewModel item)
    {
        await _orderService.UpdateItemQuantityAsync(item.OrderItem, item.OrderItem.Quantity + 1);
        RefreshOrderDisplay();
    }

    [RelayCommand]
    private async Task DecreaseQuantity(OrderItemViewModel item)
    {
        await _orderService.UpdateItemQuantityAsync(item.OrderItem, item.OrderItem.Quantity - 1);
        RefreshOrderDisplay();
    }

    [RelayCommand]
    private async Task RemoveOrderItem(OrderItemViewModel item)
    {
        await _orderService.RemoveItemAsync(item.OrderItem);
        RefreshOrderDisplay();
    }

    [RelayCommand]
    private async Task OpenModifiers(OrderItemViewModel item)
    {
        if (!item.OrderItem.MenuItem.CanHaveModifiers) return;

        ModifierTargetItem = item;
        var modifiers = await _menuService.GetModifiersForItemAsync(item.OrderItem.MenuItemId);

        AvailableModifiers = new ObservableCollection<ModifierViewModel>(
            modifiers.Select(m => new ModifierViewModel(m)
            {
                IsSelected = item.OrderItem.Modifiers.Any(om => om.ModifierId == m.Id)
            }));

        ShowModifierDialog = true;
    }

    [RelayCommand]
    private async Task ToggleModifier(ModifierViewModel modifier)
    {
        if (ModifierTargetItem == null) return;

        modifier.IsSelected = !modifier.IsSelected;

        if (modifier.IsSelected)
        {
            await _orderService.AddModifierToItemAsync(ModifierTargetItem.OrderItem, modifier.Modifier);
        }
        else
        {
            var existingModifier = ModifierTargetItem.OrderItem.Modifiers
                .FirstOrDefault(m => m.ModifierId == modifier.Modifier.Id);
            if (existingModifier != null)
            {
                await _orderService.RemoveModifierFromItemAsync(ModifierTargetItem.OrderItem, existingModifier);
            }
        }

        RefreshOrderDisplay();
    }

    [RelayCommand]
    private void CloseModifiers()
    {
        ShowModifierDialog = false;
        ModifierTargetItem = null;
        AvailableModifiers.Clear();
    }

    [RelayCommand]
    private void OpenVoidDialog(OrderItemViewModel item)
    {
        SelectedOrderItem = item;
        VoidPin = string.Empty;
        VoidReason = "Customer request";
        ShowVoidDialog = true;
    }

    [RelayCommand]
    private void AppendVoidPin(string digit)
    {
        if (VoidPin.Length < 4)
        {
            VoidPin += digit;
        }
    }

    [RelayCommand]
    private void ClearVoidPin()
    {
        VoidPin = string.Empty;
    }

    [RelayCommand]
    private async Task ConfirmVoid()
    {
        if (SelectedOrderItem == null || VoidPin.Length != 4) return;

        var success = await _orderService.VoidItemAsync(
            SelectedOrderItem.OrderItem,
            VoidReason,
            VoidPin);

        if (success)
        {
            ShowVoidDialog = false;
            RefreshOrderDisplay();
        }
        else
        {
            VoidPin = string.Empty;
            // Show error - invalid manager PIN
        }
    }

    [RelayCommand]
    private void CancelVoid()
    {
        ShowVoidDialog = false;
        VoidPin = string.Empty;
        VoidReason = string.Empty;
    }

    [RelayCommand]
    private void OpenPayment()
    {
        if (!OrderItems.Any()) return;

        TenderedAmount = string.Empty;
        TenderedAmountDisplay = "$0.00";
        ChangeAmount = 0;
        IsCardPayment = false;
        PaymentError = string.Empty;
        HasPaymentError = false;
        ShowPaymentDialog = true;
    }

    [RelayCommand]
    private void AppendTenderAmount(string digit)
    {
        // Handle decimal point
        if (digit == ".")
        {
            if (TenderedAmount.Contains('.')) return; // Only one decimal allowed
            if (string.IsNullOrEmpty(TenderedAmount))
                TenderedAmount = "0.";
            else
                TenderedAmount += ".";
        }
        else
        {
            // Limit decimal places to 2
            var decimalIndex = TenderedAmount.IndexOf('.');
            if (decimalIndex >= 0 && TenderedAmount.Length - decimalIndex > 2)
                return; // Already have 2 decimal places

            TenderedAmount += digit;
        }

        UpdateTenderedDisplay();
        CalculateChange();
        ClearPaymentError();
    }

    [RelayCommand]
    private void BackspaceTenderAmount()
    {
        if (!string.IsNullOrEmpty(TenderedAmount))
        {
            TenderedAmount = TenderedAmount[..^1];
            UpdateTenderedDisplay();
            CalculateChange();
            ClearPaymentError();
        }
    }

    [RelayCommand]
    private void ClearTenderAmount()
    {
        TenderedAmount = string.Empty;
        TenderedAmountDisplay = "$0.00";
        ChangeAmount = 0;
        ClearPaymentError();
    }

    [RelayCommand]
    private void QuickTender(string amount)
    {
        TenderedAmount = amount;
        UpdateTenderedDisplay();
        CalculateChange();
        ClearPaymentError();
    }

    private void UpdateTenderedDisplay()
    {
        if (decimal.TryParse(TenderedAmount, out var amount))
        {
            TenderedAmountDisplay = $"${amount:F2}";
        }
        else if (string.IsNullOrEmpty(TenderedAmount))
        {
            TenderedAmountDisplay = "$0.00";
        }
        else
        {
            // Handle partial input like "5."
            TenderedAmountDisplay = $"${TenderedAmount}";
        }
    }

    private void CalculateChange()
    {
        if (decimal.TryParse(TenderedAmount, out var tendered))
        {
            ChangeAmount = Math.Round(Math.Max(0, tendered - Total), 2, MidpointRounding.ToEven);
        }
        else
        {
            ChangeAmount = 0;
        }
    }

    private void ClearPaymentError()
    {
        PaymentError = string.Empty;
        HasPaymentError = false;
    }

    private void SetPaymentError(string message)
    {
        PaymentError = message;
        HasPaymentError = true;
    }

    [RelayCommand]
    private async Task PayCash()
    {
        if (string.IsNullOrEmpty(TenderedAmount))
        {
            SetPaymentError("Please enter an amount");
            return;
        }

        if (!decimal.TryParse(TenderedAmount, out var tendered))
        {
            SetPaymentError("Invalid amount entered");
            return;
        }

        if (tendered < Total)
        {
            var shortfall = Total - tendered;
            SetPaymentError($"Insufficient payment. Short ${shortfall:F2}");
            return;
        }

        var order = await _orderService.CompletePaymentAsync(PaymentMethod.Cash, tendered);
        CompletedOrder = order;
        ShowPaymentDialog = false;
        ShowReceiptDialog = true;
        RefreshOrderDisplay();
    }

    [RelayCommand]
    private async Task PayCard()
    {
        var order = await _orderService.CompletePaymentAsync(PaymentMethod.Card, Total);
        CompletedOrder = order;
        ShowPaymentDialog = false;
        ShowReceiptDialog = true;
        RefreshOrderDisplay();
    }

    [RelayCommand]
    private void CancelPayment()
    {
        ShowPaymentDialog = false;
        TenderedAmount = string.Empty;
    }

    [RelayCommand]
    private void CloseReceipt()
    {
        ShowReceiptDialog = false;
        CompletedOrder = null;
    }

    [RelayCommand]
    private async Task SendToKitchen()
    {
        if (!OrderItems.Any()) return;

        try
        {
            await _orderService.SendToKitchenAsync();
            RefreshOrderDisplay();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
    }

    [RelayCommand]
    private void ClearOrder()
    {
        _orderService.ClearCurrentOrder();
        RefreshOrderDisplay();
    }

    private void OnOrderUpdated(object? sender, Order order)
    {
        RefreshOrderDisplay();
    }

    private void RefreshOrderDisplay()
    {
        var currentOrder = _orderService.CurrentOrder;

        OrderItems = new ObservableCollection<OrderItemViewModel>(
            currentOrder.Items
                .Where(i => !i.IsVoided)
                .Select(i => new OrderItemViewModel(i)));

        Subtotal = currentOrder.Subtotal;
        Tax = currentOrder.TaxAmount;
        Total = currentOrder.Total;
        ItemCount = currentOrder.Items.Count(i => !i.IsVoided);
    }
}

// Supporting ViewModels
public partial class CategoryViewModel : ObservableObject
{
    public MenuCategory Category { get; }
    public string Name { get; }
    public string Color { get; }

    public CategoryViewModel(MenuCategory category, string name, string color)
    {
        Category = category;
        Name = name;
        Color = color;
    }
}

public partial class MenuItemViewModel : ObservableObject
{
    public MenuItem MenuItem { get; }
    public string Name => MenuItem.Name;
    public string Price => MenuItem.IsSizable
        ? $"${MenuItem.SmallPrice:F2}+"
        : $"${MenuItem.BasePrice:F2}";
    public string BackgroundColor => MenuItem.BackgroundColor;

    public MenuItemViewModel(MenuItem menuItem)
    {
        MenuItem = menuItem;
    }
}

public partial class OrderItemViewModel : ObservableObject
{
    public OrderItem OrderItem { get; }
    public string DisplayName => OrderItem.DisplayName;
    public int Quantity => OrderItem.Quantity;
    public string Price => $"${OrderItem.LineTotal:F2}";
    public string Modifiers => OrderItem.ModifierSummary;
    public bool HasModifiers => !string.IsNullOrEmpty(Modifiers);
    public bool IsCombo => OrderItem.IsCombo;
    public bool CanModify => OrderItem.MenuItem?.CanHaveModifiers ?? false;

    public OrderItemViewModel(OrderItem orderItem)
    {
        OrderItem = orderItem;
    }
}

public partial class ModifierViewModel : ObservableObject
{
    public Modifier Modifier { get; }
    public string Name => Modifier.Name;
    public string PriceDisplay => Modifier.PriceAdjustment > 0
        ? $"+${Modifier.PriceAdjustment:F2}"
        : string.Empty;
    public bool IsAddition => Modifier.IsAddition;

    [ObservableProperty]
    private bool _isSelected;

    public ModifierViewModel(Modifier modifier)
    {
        Modifier = modifier;
    }
}
