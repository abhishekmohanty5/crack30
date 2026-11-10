namespace PrepHub.Core.Entities;

/// <summary>
/// Stored refresh token for JWT authentication.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }

    /// <summary>Whether this token is still usable.</summary>
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
