namespace PrepHub.Core.Entities;

/// <summary>
/// A user's bookmark on any content item.
/// </summary>
public class Bookmark
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
