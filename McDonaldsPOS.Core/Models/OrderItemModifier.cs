namespace McDonaldsPOS.Core.Models;

/// <summary>
/// A modifier applied to a specific order item
/// </summary>
public class OrderItemModifier
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public virtual OrderItem OrderItem { get; set; } = null!;

    public int ModifierId { get; set; }
    public virtual Modifier Modifier { get; set; } = null!;

    // Snapshot values at time of order
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}
