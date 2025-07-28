namespace BugTrackingSystem.BL;

public interface IProjectManager
{
    Task<GeneralResult> AddProjectAsync(ProjectAddDto projectAddDto);
    Task<PagedResult<List<ProjectViewDto>>> GetAllProjectsAsync(int pageNumber, int pageSize);
    Task<GeneralResult> GetProjectByIdAsync(Guid id);
    Task<GeneralResult> UpdateProjectAsync(Guid id, ProjectUpdateDto projectUpdateDto);
    Task<GeneralResult> DeleteProjectAsync(Guid id);
    Task<GeneralResult> GetProjectByUser(Guid id);
    Task<GeneralResult> AssginManagerToProjectAsync(Guid projectId, Guid userId);
    Task<GeneralResult> UnAssignMemberFromProjectAsync(Guid projectId, Guid userId);
}

