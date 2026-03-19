using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkHub.Models.Models;

namespace WorkHub.Business.Service.IService
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string title, string message, string type, List<int> userIds);
        Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int userId, Guid notificationId);
    }
}
