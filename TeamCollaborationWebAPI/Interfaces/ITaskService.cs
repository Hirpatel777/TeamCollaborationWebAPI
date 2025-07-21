using TeamCollaborationWebAPI.Models;

namespace TeamCollaborationWebAPI.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetTasksForUserAsync(string userId);
        Task<TaskItem> AddTaskAsync(TaskItem task);
    }
}
