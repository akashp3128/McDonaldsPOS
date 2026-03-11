namespace McDonaldsPOS.Core.Enums;

/// <summary>
/// User roles for access control
/// </summary>
public enum UserRole
{
    Crew = 0,       // Standard cashier - basic functions
    Manager = 1     // Can void items, access reports
}
