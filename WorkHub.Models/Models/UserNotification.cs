using System;

namespace WorkHub.Models.Models;

public partial class UserNotification
{
    public int UserId { get; set; }

    public Guid NotificationId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
