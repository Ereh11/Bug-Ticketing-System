using BugTrackingSystem.DAL;

namespace BugTrackingSystem.BL;

public class ProjectManager : IProjectManager
{
    private readonly IUnitWork _unitWork;
    private readonly ProjectAddDtoValidator _projectAddDtoValidator;
    private readonly ProjectUpdateDtoValidator _projectUpdateDtoValidator;
    public ProjectManager(IUnitWork unitWork,
        ProjectAddDtoValidator projectAddDtoValidator,
        ProjectUpdateDtoValidator projectUpdateDtoValidator)
    {
        _unitWork = unitWork;
        _projectAddDtoValidator = projectAddDtoValidator;
        _projectUpdateDtoValidator = projectUpdateDtoValidator;
    }
    public async Task<GeneralResult> AddProjectAsync(ProjectAddDto projectAddDto)
    {
        var validationResult = await _projectAddDtoValidator
            .ValidateAsync(projectAddDto);
        if (!validationResult.IsValid)
        {
            return validationResult.MapErrorToGeneralResult();
        }
        var projectAddDb = new Project
        {
            Name = projectAddDto.Name,
            Description = projectAddDto.Description,
            Status = projectAddDto.Status,
            StartDate = projectAddDto.StartDate,
            EndDate = projectAddDto.EndDate,
            IsActive = projectAddDto.IsActive
        };
        await _unitWork.ProjectRepository
            .AddAsync(projectAddDb);
        await _unitWork.SaveChangesAsync();
        return new GeneralResult<ProjectViewDto>
        {
            Success = true,
            Message = "Project added successfully",
            Data = projectAddDb.MapToProjectViewDto()
        };
    }

    public async Task<GeneralResult> AssginManagerToProjectAsync(Guid projectId, Guid userId)
    {
        var projectFromDb = await _unitWork.ProjectRepository
            .GetByIdAsync(projectId);
        if (projectFromDb == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Project not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Project not found",
                        Code = "404"
                    }
                }
            };
        }
        var userFromDb = await _unitWork.UserRepository
            .GetByIdAsync(userId);
        if (userFromDb == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "User not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "User not found",
                        Code = "404"
                    }
                }
            };
        }
        projectFromDb.Manager = userFromDb;
        await _unitWork.SaveChangesAsync();
        return new GeneralResult<ProjectViewDto>
        {
            Success = true,
            Message = "Manager assigned to project successfully",
            Data = null
        };
    }

    public async Task<GeneralResult> DeleteProjectAsync(Guid id)
    {
        var projectFromDb = await _unitWork.ProjectRepository
            .GetByIdAsync(id);
        if (projectFromDb == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Project not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Project not found",
                        Code = "404"
                    }
                }
            };
        }
        await _unitWork.ProjectRepository.DeleteAsync(id);
        await _unitWork.SaveChangesAsync();
        return new GeneralResult
        {
            Success = true,
            Message = "Project deleted successfully"
        };
    }

    public async Task<PagedResult<List<ProjectViewDto>>> GetAllProjectsAsync(int pageNumber, int pageSize)
    {
        // Validate inputs
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => pageSize
        };

        var (projects, totalRecords) = await _unitWork.ProjectRepository
            .GetPaginatedProjectsWithAllInfoAsync(pageNumber, pageSize);

        if (!projects.Any())
        {
            return new PagedResult<List<ProjectViewDto>>
            {
                Success = false,
                Message = "No projects found",
                Errors = new List<ResultError> { new() { Message = "No projects found", Code = "404" } },
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

        return new PagedResult<List<ProjectViewDto>>
        {
            Success = true,
            Message = "Projects retrieved successfully",
            Data = projects.Select(p => p.MapToProjectViewDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task<GeneralResult> GetProjectByIdAsync(Guid id)
    {
        var projectFromDb = await _unitWork.ProjectRepository
            .GetProjectWithAllInfoAsync(id);
        if (projectFromDb == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Project not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Project not found",
                        Code = "404"
                    }
                }
            };
        }
        return new GeneralResult<ProjectViewDto>
        {
            Success = true,
            Message = "Projects found",
            Data = projectFromDb.MapToProjectViewDto()
        };
    }

    public async Task<GeneralResult> GetProjectByUser(Guid id)
    {
        var projects = await _unitWork.ProjectRepository
            .GetProjectsByUserIdAsync(id);
        if (projects == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Projects not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Projects not found",
                        Code = "404"
                    }
                }
            };
        }
        return new GeneralResult<List<ProjectViewDto>>
        {
            Success = true,
            Message = "Projects found",
            Data = projects.Select(p => p.MapToUserProjectViewDto()).ToList()
        };  

    }

    public async Task<GeneralResult> UnAssignMemberFromProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _unitWork.ProjectRepository
            .GetByIdAsync(projectId);
        if (project == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Project not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Project not found",
                        Code = "404"
                    }
                }
            };
        }
        var user = await _unitWork.UserRepository
            .GetByIdAsync(userId);
        if (user == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "User not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "User not found",
                        Code = "404"
                    }
                }
            };
        }
        bool checkExistance = await _unitWork.ProjectMemberRepository
            .ExistsByCompositeKeyAsync(projectId, userId);
        if (!checkExistance)
        {   return new GeneralResult
            {
                Success = false,
                Message = "User is not assigned to this project",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "User is not assigned to this project",
                        Code = "400"
                    }
                }
            };
        }
        await _unitWork.ProjectMemberRepository
            .RemoveProjectMemberAsync(projectId, userId);
        await _unitWork.SaveChangesAsync();
        return new GeneralResult
        {
            Success = true,
            Message = "User unassigned from project successfully"
        };
    }
    

    public async Task<GeneralResult> UpdateProjectAsync(Guid id, ProjectUpdateDto projectUpdateDto)
    {
        var projectFromDb = await _unitWork.ProjectRepository
            .GetByIdAsync(id);
        if (projectFromDb == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Project not found",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Project not found",
                        Code = "404"
                    }
                }
            };
        }
        var validationResult = await _projectUpdateDtoValidator
            .ValidateAsync(projectUpdateDto);
        if (!validationResult.IsValid)
        {
            return validationResult.MapErrorToGeneralResult();
        }
        projectFromDb.Name = projectUpdateDto.Name;
        projectFromDb.Description = projectUpdateDto.Description;
        //projectFromDb.Status = projectUpdateDto.Status;
        //projectFromDb.StartDate = projectUpdateDto.StartDate;
        //projectFromDb.EndDate = projectUpdateDto.EndDate;
        //projectFromDb.IsActive = projectUpdateDto.IsActive;
        await _unitWork.SaveChangesAsync();
        return new GeneralResult<ProjectViewDto>
        {
            Success = true,
            Message = "Project updated successfully",
            
        };
    }
}
