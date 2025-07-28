using System;

namespace BugTrackingSystem.BL;

public class CommentViewDto
{
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime TextDate { get; set; } = DateTime.UtcNow;

}