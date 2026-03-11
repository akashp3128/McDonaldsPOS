using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using McDonaldsPOS.Data.Repositories;

namespace McDonaldsPOS.Services;

/// <summary>
/// Service for menu operations
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    // Cache default combo items (medium fries, medium coke)
    private MenuItem? _defaultSide;
    private MenuItem? _defaultDrink;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync()
    {
        return await _menuRepository.GetActiveItemsAsync();
    }

    public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(MenuCategory category)
    {
        return await _menuRepository.GetByCategoryAsync(category);
    }

    public async Task<MenuItem?> GetMenuItemAsync(int id)
    {
        return await _menuRepository.GetWithModifiersAsync(id);
    }

    public async Task<IEnumerable<Modifier>> GetModifiersForItemAsync(int menuItemId)
    {
        return await _menuRepository.GetModifiersForItemAsync(menuItemId);
    }

    public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
    {
        return await _menuRepository.GetAllModifiersAsync();
    }

    public async Task<(MenuItem? Side, MenuItem? Drink)> GetDefaultComboItemsAsync(MenuItem mainItem)
    {
        // Cache the default items for performance
        if (_defaultSide == null || _defaultDrink == null)
        {
            var allItems = await _menuRepository.GetActiveItemsAsync();

            // Default side: French Fries
            _defaultSide = allItems.FirstOrDefault(i =>
                i.Category == MenuCategory.FriesSides && i.Name.Contains("French Fries"));

            // Default drink: Coca-Cola
            _defaultDrink = allItems.FirstOrDefault(i =>
                i.Category == MenuCategory.Drinks && i.Name.Contains("Coca-Cola"));
        }

        return (_defaultSide, _defaultDrink);
    }

    public decimal GetPriceForSize(MenuItem item, ItemSize size)
    {
        if (!item.IsSizable)
            return item.BasePrice;

        return size switch
        {
            ItemSize.Small => item.SmallPrice,
            ItemSize.Medium => item.MediumPrice,
            ItemSize.Large => item.LargePrice,
            _ => item.BasePrice
        };
    }
}
