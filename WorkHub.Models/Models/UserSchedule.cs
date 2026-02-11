using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class UserSchedule
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
