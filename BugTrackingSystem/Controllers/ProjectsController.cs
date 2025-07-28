using BugTrackingSystem.BL;
using BugTrackingSystem.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectManager _projectManager;
        public ProjectsController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        [HttpGet]
        public async Task<Results<Ok<PagedResult<List<ProjectViewDto>>>, NotFound<PagedResult<List<ProjectViewDto>>>>>
    GetAllProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _projectManager.GetAllProjectsAsync(pageNumber, pageSize);

            return response.Success
                ? TypedResults.Ok(response)
                : TypedResults.NotFound(response);
        }
        [HttpPost]
        //[Authorize(Policy = Constants.Policies.Admin)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> AddProject([FromBody] ProjectAddDto projectAddDto)
        {
            var response = await _projectManager
                .AddProjectAsync(projectAddDto);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest(response);
            }
        }
        [HttpGet("{id:guid}")]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> GetProjectById(Guid id)
        {
            var project = await _projectManager
                .GetProjectByIdAsync(id);
            if (project.Success)
            {
                return TypedResults.Ok(project);
            }
            else
            {
                return TypedResults.BadRequest(project);
            }
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Constants.Policies.Admin)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> DeleteProject(Guid id)
        {
            var response = await _projectManager
                .DeleteProjectAsync(id);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest(response);
            }
        }
        [HttpPut("{id:guid}")]
        //[Authorize(Policy = Constants.Policies.Admin)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> UpdateProject(Guid id, [FromBody] ProjectUpdateDto projectUpdateDto)
        {
            var response = await _projectManager
                .UpdateProjectAsync(id, projectUpdateDto);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest(response);
            }
        }
        [HttpGet("user/{id:guid}")]
        public async Task<Results<Ok<GeneralResult>, NotFound<GeneralResult>>> GetProjectByUser(Guid id)
        {
            var response = await _projectManager.GetProjectByUser(id);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.NotFound(response);
            }
        }
        [HttpPost("{projectId:guid}/manager/{userId:guid}")]
        [Authorize(Policy = Constants.Policies.Admin)]
        public async Task<Results<Ok<GeneralResult>, NotFound<GeneralResult>>> AssignManagerToProject(Guid projectId, Guid userId)
        {
            var response = await _projectManager
                .AssginManagerToProjectAsync(projectId, userId);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.NotFound(response);
            }
        }
        [HttpDelete("{projectId:guid}/member/{userId:guid}")]
        //[Authorize(Policy = Constants.Policies.Admin)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> UnassignMemberFromProject(Guid projectId, Guid userId)
        {
            var response = await _projectManager
                .UnAssignMemberFromProjectAsync(projectId, userId);
            if (response.Success)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest(response);
            }
        }

    }
}
