using BugTrackingSystem.DAL;

namespace BugTrackingSystem.BL;

public class BugManager : IBugManager
{
    private readonly IUnitWork _unitWork;
    private readonly AddBugDtoValidator _addBugDtoValidator;
    private readonly BugUpdateDtoValidator _bugUpdateDtoValidator;
    public BugManager(
        IUnitWork unitWork,
        AddBugDtoValidator addBugDtoValidator,
        BugUpdateDtoValidator bugUpdateDtoValidator
        )
    {
        _unitWork = unitWork;
        _addBugDtoValidator = addBugDtoValidator;
        _bugUpdateDtoValidator = bugUpdateDtoValidator;
    }
    public async Task<GeneralResult> AddBugAsync(BugAddDto bugAddDto)
    {
        var validationResult = await _addBugDtoValidator
             .ValidateAsync(bugAddDto);
        if (!validationResult.IsValid)
        {
            return validationResult.MapErrorToGeneralResult();
        }
        var bug = new Bug
        {
            Title = bugAddDto.Title,
            Description = bugAddDto.Description,
            Status = bugAddDto.Status,
            Priority = bugAddDto.Priority,
            ProjectId = bugAddDto.ProjectId,
        };
        await _unitWork.BugRepository
            .AddAsync(bug);
        await _unitWork.SaveChangesAsync();
        return new GeneralResult
        {
            Success = true,
            Message = "Bug added successfully.",
        };
    }

    public async Task<GeneralResult> GetAllBugsAsync()
    {
        var bugs = await _unitWork.BugRepository
            .GetBugWithProjectAndAttachmentInfo();
        if (bugs == null || !bugs.Any())
        {
            return new GeneralResult
            {
                Success = false,
                Message = "No bugs found."
            };
        }
        var bugViewDtos = bugs.Select(b => new BugViewDto
        {
            Id = b.BugId,
            Title = b.Title,
            Description = b.Description,
            Status = b.Status,
            Priority = b.Priority,
            Project = new ProjectViewForBugDto
            {
                ProjectId = b.Project.ProjectId,
                Name = b.Project.Name,
            },
            Attachments = b.Attachments.Select(a => new AttachmentForBugDto
            {
                AttachmentId = a.AttachmentId,
                FileName = a.FileName,
                FilePath = a.FileUrl,
                CreatedDate = a.CreatedDate,
            }).ToList(),
            BugAssignmentUser = b.BugAssignments.Select(ba => new BugAssignmentUserDto
            {
                UserName = ba.User.FirstName + " " + ba.User.LastName
            }).ToList()
        }).ToList();

        return new GeneralResult<List<BugViewDto>>
        {
            Success = true,
            Message = "Bugs retrieved successfully.",
            Data = bugViewDtos
        };
    }

    public async Task<GeneralResult> GetAssigneesByBugIdAsync(Guid bugId)
    {
        var bug = await _unitWork.BugRepository
            .GetByIdAsync(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found."
            };
        }
        var assignees = await _unitWork.BugAssignmentRepository
            .GetUserAssignmentsByBugIdAsync(bugId);
        if (assignees == null || !assignees.Any())
        {
            return new GeneralResult
            {
                Success = false,
                Message = "No assignees found for this bug."
            };
        }
        var assigneeDtos = assignees.Select(a => new BugAssignmentViewDto
        {
            BugId = a.BugId,
            UserId = a.UserId,
            AssignedDate = a.AssignedDate
        });
        return new GeneralResult<List<BugAssignmentViewDto>>
        {
            Success = true,
            Message = "Assignees retrieved successfully.",
            Data = assigneeDtos.ToList()
        };
    }

    public async Task<GeneralResult> GetBugByIdAsync(Guid bugId)
    {
        var bug = await _unitWork.BugRepository
            .GetBugWithProjectAndAttachmentInfo(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found."
            };
        }
        var bugViewDto = new BugViewDto
        {
            Id = bug.BugId,
            Title = bug.Title,
            Description = bug.Description,
            Status = bug.Status,
            Priority = bug.Priority,
            Project = new ProjectViewForBugDto
            {
                ProjectId = bug.Project.ProjectId,
                Name = bug.Project.Name,
            },
            Attachments = bug.Attachments.Select(a => new AttachmentForBugDto
            {
                AttachmentId = a.AttachmentId,
                FileName = a.FileName,
                FilePath = a.FileUrl,
                CreatedDate = a.CreatedDate,
            }).ToList(),
            BugAssignmentUser = bug.BugAssignments.Select(ba => new BugAssignmentUserDto
            {
                UserName = ba.User.FirstName + " " + ba.User.LastName
            }).ToList()
        };
        return new GeneralResult<BugViewDto>
        {
            Success = true,
            Message = "Bug retrieved successfully.",
            Data = bugViewDto
        };
    }

    public async Task<GeneralResult> RemoveBugAsync(Guid bugId)
    {
        var bug = await _unitWork.BugRepository
            .GetByIdAsync(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found."
            };
        }
        await _unitWork.BugRepository
            .DeleteAsync(bugId);
        await _unitWork.SaveChangesAsync();
        return new GeneralResult
        {
            Success = true,
            Message = "Bug removed successfully."
        };
    }
    public async Task<GeneralResult> UpdateBugAsync(Guid bugId, BugUpdateDto bugUpdateDto)
    {
        var validationResult = await _bugUpdateDtoValidator
            .ValidateAsync(bugUpdateDto);
        if (!validationResult.IsValid)
        {
            return validationResult.MapErrorToGeneralResult();
        }
        var bug = await _unitWork.BugRepository
            .GetByIdAsync(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found."
            };
        }

        var addBugToUserDto = bugUpdateDto.ChangeAssignmentDto;
        if (addBugToUserDto != null && addBugToUserDto?.NewUserId != Guid.Empty)
        {
            if (addBugToUserDto?.OldUserId != null && addBugToUserDto.OldUserId != Guid.Empty)
            {
                // Remove old assignment if exists
                await _unitWork.BugAssignmentRepository
                    .RemoveBugAssignmentAsync(bugId, addBugToUserDto.OldUserId.Value);
            }
            var bugAssignment = new BugAssignment
            {
                BugId = bugId,
                UserId = addBugToUserDto.NewUserId,
                AssignedDate = DateTime.UtcNow
            };
            await _unitWork.BugAssignmentRepository
                .AddAsync(bugAssignment);
            await _unitWork.SaveChangesAsync();
        }
        bug.Title = bugUpdateDto.Title;
        bug.Description = bugUpdateDto.Description;
        bug.Status = bugUpdateDto.Status;
        bug.Priority = bugUpdateDto.Priority;
        await _unitWork.SaveChangesAsync();
        return new GeneralResult
        {
            Success = true,
            Message = "Bug updated successfully.",
        };
    }
    public async Task<GeneralResult> GetBugByIdWithAllInfoAsync(Guid bugId)
    {
        var bug = await _unitWork.BugRepository
            .GetBugByIdWithProjectAndAttachmentInfo(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found."
            };
        }
        var bugViewDto = new BugViewDto
        {
            Id = bug.BugId,
            Title = bug.Title,
            Description = bug.Description,
            Status = bug.Status,
            Priority = bug.Priority,
            Project = new ProjectViewForBugDto
            {
                ProjectId = bug.Project.ProjectId,
                Name = bug.Project.Name,
            },
            Attachments = bug.Attachments.Select(a => new AttachmentForBugDto
            {
                AttachmentId = a.AttachmentId,
                FileName = a.FileName,
                FilePath = a.FileUrl,
                CreatedDate = a.CreatedDate,
            }).ToList(),
            BugAssignmentUser = bug.BugAssignments.Select(ba => new BugAssignmentUserDto
            {
                UserName = ba.User.FirstName + " " + ba.User.LastName
            }).ToList()
        };
        return new GeneralResult<BugViewDto>
        {
            Success = true,
            Message = "Bug retrieved successfully.",
            Data = bugViewDto
        };
    }
}
