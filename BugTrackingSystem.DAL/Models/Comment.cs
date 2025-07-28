namespace BugTrackingSystem.DAL;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime TextDate { get; set; } = DateTime.UtcNow; 
    public Guid BugId { get; set; }
    public Bug Bug { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
