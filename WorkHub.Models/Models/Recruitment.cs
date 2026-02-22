using System;
using System.Collections.Generic;


namespace WorkHub.Models.Models;

public partial class Recruitment
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int? PostId { get; set; }

    public string? JobName { get; set; }

    public int CategoryId { get; set; }
    public int JobTypeId { get; set; }

    public Category Category { get; set; } = null!;
    public JobType JobType { get; set; } = null!;

    public string? Location { get; set; }
    public string? Salary { get; set; }
    public string? Status { get; set; }

    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? WorkTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } 

    public virtual Post Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

