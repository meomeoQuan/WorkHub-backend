using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class Recruitment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? JobName { get; set; }

    public string? JobType { get; set; }

    public string? Location { get; set; }

    public string? Salary { get; set; }

    public string? Status { get; set; }

    public string? Schedule { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual User User { get; set; } = null!;
}
