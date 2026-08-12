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
    public class ProjectMembersController : ControllerBase
    {
        private readonly ProjectMemberRepository _memberRepository;
        private readonly ProjectRepository _projectRepository;

        public ProjectMembersController(ProjectMemberRepository memberRepository, ProjectRepository projectRepository)
        {
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
        }

        // GET: api/projectmembers  (everyone — used to build the Teams page)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _memberRepository.GetAll();
            return Ok(members);
        }

        // GET: api/projectmembers/project/5
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var members = await _memberRepository.GetByProject(projectId);
            return Ok(members);
        }

        // POST: api/projectmembers
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Add([FromBody] AddProjectMemberRequest request)
        {
            if (!await CanManage(request.ProjectId)) return Forbid();
            await _memberRepository.AddMember(request);
            return Ok();
        }

        // DELETE: api/projectmembers/5/12  (projectId/userId)
        [HttpDelete("{projectId}/{userId}")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Remove(int projectId, int userId)
        {
            if (!await CanManage(projectId)) return Forbid();
            await _memberRepository.RemoveMember(projectId, userId);
            return NoContent();
        }

        // Admin can manage any team; a Project Manager only their own project
        private async Task<bool> CanManage(int projectId)
        {
            if (User.IsInRole("Admin")) return true;
            var project = await _projectRepository.GetProjectById(projectId);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return project != null && project.CreatedBy == userId;
        }
    }
}