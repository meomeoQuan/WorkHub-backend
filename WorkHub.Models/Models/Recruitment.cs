using System;
using System.Collections.Generic;
using WorkHub.Models.Enums;

namespace WorkHub.Models.Models;

public partial class Recruitment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }   // 👈 ADD THIS

    public string? JobName { get; set; }

    public JobType JobType { get; set; }

    public string? Location { get; set; }

    public string? Salary { get; set; }

    public string? Status { get; set; }

   
    public string? ExperienceLevel { get; set; }
    public string? Category { get; set; }
    public string? WorkSetting { get; set; }
    public string? CompanySize { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Post Post { get; set; } = null!;   // 👈 ADD THIS

    public virtual User User { get; set; } = null!;
}
