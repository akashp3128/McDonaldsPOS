using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace McDonaldsPOS.Data.Repositories;

/// <summary>
/// Repository for menu-related data operations
/// </summary>
public class MenuRepository : Repository<MenuItem>, IMenuRepository
{
    public MenuRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuCategory category)
    {
        return await _dbSet
            .Where(m => m.Category == category && m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<MenuItem>> GetActiveItemsAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .OrderBy(m => m.Category)
            .ThenBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<MenuItem?> GetWithModifiersAsync(int id)
    {
        return await _dbSet
            .Include(m => m.AvailableModifiers)
            .ThenInclude(mm => mm.Modifier)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
    {
        return await _context.Modifiers
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Modifier>> GetModifiersForItemAsync(int menuItemId)
    {
        return await _context.MenuItemModifiers
            .Where(mm => mm.MenuItemId == menuItemId)
            .Include(mm => mm.Modifier)
            .Select(mm => mm.Modifier)
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }
}
