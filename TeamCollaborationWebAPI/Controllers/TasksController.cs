using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamCollaborationWebAPI.Interfaces;
using TeamCollaborationWebAPI.Models;

namespace TeamCollaborationWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TasksController(ITaskService taskService, IHttpContextAccessor httpContextAccessor)
        {
            _taskService = taskService;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskItem task)
        {
            task.AssignedToUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var created = await _taskService.AddTaskAsync(task);

            return Ok(created);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await _taskService.GetTasksForUserAsync(userId);
            return Ok(tasks);
        }
    }
}
