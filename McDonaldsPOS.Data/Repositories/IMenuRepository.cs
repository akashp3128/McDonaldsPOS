using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository interface for menu-related operations
/// </summary>
public interface IMenuRepository : IRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuCategory category);
    Task<IEnumerable<MenuItem>> GetActiveItemsAsync();
    Task<MenuItem?> GetWithModifiersAsync(int id);
    Task<IEnumerable<Modifier>> GetAllModifiersAsync();
    Task<IEnumerable<Modifier>> GetModifiersForItemAsync(int menuItemId);
}
