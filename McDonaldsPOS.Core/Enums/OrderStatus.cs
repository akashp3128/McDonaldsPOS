namespace McDonaldsPOS.Core.Enums;

/// <summary>
/// Status of an order through the kitchen workflow
/// </summary>
public enum OrderStatus
{
    Draft = 0,      // Being built on POS
    Pending = 1,    // Sent to kitchen, waiting
    InPrep = 2,     // Being prepared
    Ready = 3,      // Ready for pickup
    Completed = 4,  // Customer received
    Voided = 5      // Cancelled/voided
}
