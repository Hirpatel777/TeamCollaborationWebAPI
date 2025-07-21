//using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client; // Ensure this namespace is included
using System; // Ensure this namespace is included for Console.WriteLine
using System.Threading.Tasks;
using TeamCollaborationWebAPI.Hubs;
using TeamCollaborationWebAPI.Models;

namespace TeamCollaborationWebAPI.Services
{
    public interface ITaskHubService
    {
        Task NotifyUser(TaskItem task);
    }
    public class TaskHubService : ITaskHubService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskHubService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUser(TaskItem task)
        {
            //if (!string.IsNullOrEmpty(task.AssignedToUserId))
            //{
            //    await _hubContext.Clients.User(task.AssignedToUserId)
            //        .SendAsync("ReceiveTask", task);
            //}
        }
    }

}

