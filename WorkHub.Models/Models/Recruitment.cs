using System;
using System.Collections.Generic;


namespace WorkHub.Models.Models;

public partial class Recruitment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? JobName { get; set; }

    public int CategoryId { get; set; }
    public int JobTypeId { get; set; }
    public int? CityId { get; set; }

    public Category Category { get; set; } = null!;
    public JobType JobType { get; set; } = null!;
    public City? City { get; set; }

    public string? Location { get; set; }
    public string? Salary { get; set; }
    public decimal MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? SalaryCurrency { get; set; } // VND, USD
    public string? SalaryCycle { get; set; }   // Hour, Day, Month, Year
    public string? Status { get; set; }

    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? WorkTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PostRecruitment> PostRecruitments { get; set; } = new List<PostRecruitment>();
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    public virtual User User { get; set; } = null!;
}

