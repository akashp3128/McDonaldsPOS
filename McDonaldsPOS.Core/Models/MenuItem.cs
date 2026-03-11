using McDonaldsPOS.Core.Enums;

namespace McDonaldsPOS.Core.Models;

/// <summary>
/// A menu item available for sale
/// </summary>
public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty; // For receipt/KDS
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public MenuCategory Category { get; set; }
    public bool IsSizable { get; set; } // Can be Small/Med/Large
    public decimal SmallPrice { get; set; }
    public decimal MediumPrice { get; set; }
    public decimal LargePrice { get; set; }
    public bool IsComboEligible { get; set; } // Can be part of a meal
    public int? DefaultSideId { get; set; } // Default side for combo (e.g., Medium Fries)
    public int? DefaultDrinkId { get; set; } // Default drink for combo (e.g., Medium Coke)
    public decimal ComboUpcharge { get; set; } // Price to make it a meal (typically 0, price difference handled by sides)
    public bool CanHaveModifiers { get; set; } = true;
    public string ImagePath { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#FFC107"; // McDonald's yellow default
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<MenuItemModifier> AvailableModifiers { get; set; } = new List<MenuItemModifier>();
}
