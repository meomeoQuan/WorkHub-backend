using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("RecruitmentInfo")]
public partial class RecruitmentInfo
{
    [Key]
    public int Id { get; set; }

    public int EmployerId { get; set; }

    public int CompanyId { get; set; }

    [StringLength(100)]
    public string JobName { get; set; } = null!;

    [StringLength(50)]
    public string JobType { get; set; } = null!;

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(50)]
    public string? Salary { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Status { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Schedule { get; set; }

    [InverseProperty("Recruitment")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("CompanyId")]
    [InverseProperty("RecruitmentInfos")]
    public virtual Company Company { get; set; } = null!;

    [ForeignKey("EmployerId")]
    [InverseProperty("RecruitmentInfos")]
    public virtual Employer Employer { get; set; } = null!;
}
