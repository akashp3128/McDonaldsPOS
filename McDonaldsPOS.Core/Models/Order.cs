using McDonaldsPOS.Core.Enums;

namespace McDonaldsPOS.Core.Models;

/// <summary>
/// A customer order
/// </summary>
public class Order
{
    public int Id { get; set; }
    public int OrderNumber { get; set; } // Display number (resets daily typically)
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Pricing
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.08m; // 8% tax rate
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    // Payment
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal AmountTendered { get; set; }
    public decimal ChangeGiven { get; set; }

    // Tracking
    public int? CreatedByUserId { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public string? VoidReason { get; set; }
    public int? VoidedByUserId { get; set; }

    // Navigation
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Recalculate order totals from items (excludes voided items)
    /// </summary>
    public void RecalculateTotals()
    {
        // Only sum non-voided items
        Subtotal = Items.Where(i => !i.IsVoided).Sum(i => i.LineTotal);
        // Use banker's rounding (MidpointRounding.ToEven) for accurate currency calculations
        TaxAmount = Math.Round(Subtotal * TaxRate, 2, MidpointRounding.ToEven);
        Total = Math.Round(Subtotal + TaxAmount, 2, MidpointRounding.ToEven);
    }
}
