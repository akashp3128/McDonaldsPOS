using McDonaldsPOS.Core.Enums;

namespace McDonaldsPOS.Core.Models;

/// <summary>
/// POS user account (crew member or manager)
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty; // 4-digit PIN (hashed in production)
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
