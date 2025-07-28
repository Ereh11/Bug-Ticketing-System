using System;

namespace BugTrackingSystem.BL;

public class CommentAddDto
{
    public string Text { get; set; } = string.Empty;
    public DateTime TextDate { get; set; } = DateTime.UtcNow;
    public Guid BugId { get; set; }
    public Guid UserId { get; set; }
}