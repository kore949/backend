using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;

        public TasksController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: api/tasks
        // GET: api/tasks?projectId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? projectId)
        {
            var tasks = await _taskRepository.GetAllTasks(projectId);
            return Ok(tasks);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskRepository.GetTaskById(id);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var newTaskId = await _taskRepository.CreateTask(request);
            return CreatedAtAction(nameof(GetById), new { id = newTaskId }, new { TaskId = newTaskId });
        }

        // PUT: api/tasks/5
        // Open to all logged-in users so Team Members can update status on their tasks
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            await _taskRepository.UpdateTask(id, request);
            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskRepository.DeleteTask(id);
            return NoContent();
        }
    }
}