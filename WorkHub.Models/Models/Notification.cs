using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
