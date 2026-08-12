using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectsController(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectRepository.GetAllProjects();
            return Ok(projects);
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _projectRepository.GetProjectById(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        // POST: api/projects
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            request.CreatedBy = userId;

            var newProjectId = await _projectRepository.CreateProject(request);
            return CreatedAtAction(nameof(GetById), new { id = newProjectId }, new { ProjectId = newProjectId });
        }

        // PUT: api/projects/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
        {
            await _projectRepository.UpdateProject(id, request);
            return NoContent();
        }

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectRepository.DeleteProject(id);
            return NoContent();
        }
        // PUT: api/projects/5/manager
        [HttpPut("{id}/manager")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> SetManager(int id, [FromBody] SetManagerRequest request)
        {
            await _projectRepository.SetProjectManager(id, request.UserId);
            return NoContent();
        }

        // DELETE: api/projects/5/manager
        [HttpDelete("{id}/manager")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> RemoveManager(int id, [FromBody] SetManagerRequest request)
        {
            await _projectRepository.RemoveProjectManager(id, request.UserId);
            return NoContent();
        }
        public class SetManagerRequest
        {
            public int UserId { get; set; }
        }
    }
}