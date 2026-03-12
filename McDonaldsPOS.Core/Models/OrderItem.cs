using McDonaldsPOS.Core.Enums;

namespace McDonaldsPOS.Core.Models;

/// <summary>
/// An item within an order
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int MenuItemId { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;

    public string ItemName { get; set; } = string.Empty; // Snapshot at time of order
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => IsVoided ? 0m : Math.Round(UnitPrice * Quantity + ModifierTotal, 2, MidpointRounding.ToEven);

    public ItemSize Size { get; set; } = ItemSize.None;
    public bool IsCombo { get; set; } // Part of a meal deal
    public int? ParentComboItemId { get; set; } // Links sides/drinks to main item in combo

    public bool IsVoided { get; set; }
    public int? VoidedByUserId { get; set; }
    public string? VoidReason { get; set; }

    // Modifiers applied to this item
    public virtual ICollection<OrderItemModifier> Modifiers { get; set; } = new List<OrderItemModifier>();

    public decimal ModifierTotal => Modifiers.Sum(m => m.PriceAdjustment);

    /// <summary>
    /// Get display string with size if applicable
    /// </summary>
    public string DisplayName => Size != ItemSize.None
        ? $"{Size} {ItemName}"
        : ItemName;

    /// <summary>
    /// Get modifier summary for display
    /// </summary>
    public string ModifierSummary => Modifiers.Any()
        ? string.Join(", ", Modifiers.Select(m => m.ModifierName))
        : string.Empty;
}
