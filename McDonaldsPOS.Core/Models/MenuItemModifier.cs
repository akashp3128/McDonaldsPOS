namespace McDonaldsPOS.Core.Models;

/// <summary>
/// Many-to-many relationship between MenuItems and available Modifiers
/// </summary>
public class MenuItemModifier
{
    public int MenuItemId { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;

    public int ModifierId { get; set; }
    public virtual Modifier Modifier { get; set; } = null!;
}
