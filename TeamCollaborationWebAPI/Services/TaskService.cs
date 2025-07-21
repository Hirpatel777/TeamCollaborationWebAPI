using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using TeamCollaborationWebAPI.Hubs;
using TeamCollaborationWebAPI.Interfaces;
using TeamCollaborationWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamCollaborationWebAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<TaskHub> _hubContext;
        private readonly ITaskHubService _hubService;
        public TaskService(AppDbContext context, IMemoryCache cache, IHubContext<TaskHub> hubContext, ITaskHubService hubService)
        {
            _context = context;
            _cache = cache;
            _hubContext = hubContext;
            _hubService = hubService;
        }
       
        public async Task<IEnumerable<TaskItem>> GetTasksForUserAsync(string userId)
        {
            string cacheKey = $"tasks_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<TaskItem> tasks))
            {
                tasks = await _context.Tasks
                    .Where(t => t.AssignedToUserId == userId)
                    .ToListAsync();

                _cache.Set(cacheKey, tasks, TimeSpan.FromMinutes(5));

            }

            return tasks;
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Clear and update cache
            string cacheKey = $"tasks_{task.AssignedToUserId}";
            _cache.Remove(cacheKey);
            //_cache.Set(cacheKey, task, TimeSpan.FromMinutes(5));
            // Notify user via SignalR
            //await _hubContext.Clients.User(task.AssignedToUserId).SendAsync("ReceiveTask", task);
            //await _hubContext.Clients.User(task.AssignedToUserId)
            //            .SendAsync("ReceiveTest", $"New Task: {task.Title}");
            await _hubContext.Clients.User(task.AssignedToUserId)
                  .SendAsync("ReceiveTask", task);
           // await _hubService.NotifyUser(task);
            return task;
        }
    }
}

