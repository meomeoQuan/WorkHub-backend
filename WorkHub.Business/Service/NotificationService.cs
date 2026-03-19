using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.Models;

namespace WorkHub.Business.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateNotificationAsync(string title, string message, string type, List<int> userIds)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.NotificationRepository.Add(notification);
            
            foreach (var userId in userIds)
            {
                var userNotif = new UserNotification
                {
                    NotificationId = notification.Id,
                    UserId = userId,
                    IsRead = false
                };
                _unitOfWork.UserNotificationRepository.Add(userNotif);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId)
        {
            return await _unitOfWork.UserNotificationRepository.GetAllAsync(
                filter: un => un.UserId == userId,
                includeProperties: "Notification"
            );
        }

        public async Task MarkAsReadAsync(int userId, Guid notificationId)
        {
            var userNotif = await _unitOfWork.UserNotificationRepository.GetAsync(
                un => un.UserId == userId && un.NotificationId == notificationId);
            
            if (userNotif != null && !userNotif.IsRead)
            {
                userNotif.IsRead = true;
                userNotif.ReadAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var unread = await _unitOfWork.UserNotificationRepository.GetAllAsync(
                filter: un => un.UserId == userId && !un.IsRead);
            return unread.Count();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var unreadNotifs = await _unitOfWork.UserNotificationRepository.GetAllAsync(
                filter: un => un.UserId == userId && !un.IsRead);

            foreach (var notif in unreadNotifs)
            {
                notif.IsRead = true;
                notif.ReadAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveAsync();
        }
    }
}
