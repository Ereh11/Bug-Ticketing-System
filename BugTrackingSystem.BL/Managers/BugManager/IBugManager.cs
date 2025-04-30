
namespace BugTrackingSystem.BL;

public interface IBugManager
{
    Task<GeneralResult> AddBugAsync(BugAddDto bugAddDto);
    Task<GeneralResult> RemoveBugAsync(Guid bugId);
    Task<GeneralResult> GetAllBugsAsync();
    Task<GeneralResult> GetBugByIdAsync(Guid bugId);
    Task<GeneralResult> UpdateBugAsync(Guid bugId, BugUpdateDto bugUpdateDto);
}
