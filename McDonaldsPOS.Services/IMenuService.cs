using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service interface for menu operations
/// </summary>
public interface IMenuService
{
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync();
    Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(MenuCategory category);
    Task<MenuItem?> GetMenuItemAsync(int id);
    Task<IEnumerable<Modifier>> GetModifiersForItemAsync(int menuItemId);
    Task<IEnumerable<Modifier>> GetAllModifiersAsync();

    // Get default combo items
    Task<(MenuItem? Side, MenuItem? Drink)> GetDefaultComboItemsAsync(MenuItem mainItem);

    // Price calculation helpers
    decimal GetPriceForSize(MenuItem item, ItemSize size);
}
