﻿using BugTrackingSystem.DAL;


namespace BugTrackingSystem.BL;

public static class ProjectManagerMappingFunctions
{
    public static ProjectViewDto MapToProjectViewDto(this Project project)
    {
        return new ProjectViewDto
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            Manager = project.Manager != null ? new UserViewDto
            {
                Id = project.Manager.Id,
                FirstName = project.Manager.FirstName,
                LastName = project.Manager.LastName,
                Email = project.Manager.Email

            } : null,
            Users = project.ProjectMembers.Select(pm => new UserViewInProjectInfo
            {
                Id = pm.UserId,
                FirstName = pm.User.FirstName,
                LastName = pm.User.LastName,
                Email = pm.User.Email,
                IsActive = pm.User.IsActive
            }).ToList(),
            Bugs = project.Bugs.Select(b => new BugViewForProjectInfo
            {
                Id = b.BugId,
                Title = b.Title,
                Description = b.Description,
                Status = b.Status,
                Priority = b.Priority
            }).ToList()
        };
    }
    public static ProjectViewDto MapToUserProjectViewDto(this Project project)
    {
        return new ProjectViewDto
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            Manager = project.Manager != null ? new UserViewDto
            {
                Id = project.Manager.Id,
                FirstName = project.Manager.FirstName,
                LastName = project.Manager.LastName,
                Email = project.Manager.Email

            } : null,
            Bugs = project.Bugs.Select(b => new BugViewForProjectInfo
            {
                Id = b.BugId,
                Title = b.Title,
                Description = b.Description,
                Status = b.Status,
                Priority = b.Priority
            }).ToList()
        };
    }
}
