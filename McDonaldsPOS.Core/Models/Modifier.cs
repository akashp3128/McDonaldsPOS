namespace McDonaldsPOS.Core.Models;

/// <summary>
/// Modifier that can be applied to menu items (e.g., No Pickles, Extra Cheese)
/// </summary>
public class Modifier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty; // For receipt/KDS display
    public decimal PriceAdjustment { get; set; } // Usually 0 for "no" items, positive for extras
    public bool IsAddition { get; set; } // true = "Add X", false = "No X"
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
