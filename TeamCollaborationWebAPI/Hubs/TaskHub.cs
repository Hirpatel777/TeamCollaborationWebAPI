using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TeamCollaborationWebAPI.Models;

namespace TeamCollaborationWebAPI.Hubs
{
    public class TaskHub : Hub
    {
        public async Task SendNotification(string userId, TaskItem task)
        {
            await Clients.User(userId).SendAsync("ReceiveTask", task);
        }
    }
}
