using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("Company")]
public partial class Company
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string CompanyName { get; set; } = null!;

    [StringLength(200)]
    public string? CompanyWebUrl { get; set; }

    [StringLength(200)]
    public string? CompanyLogoUrl { get; set; }

    public string CompanyIndustry { get; set; } = null!;

    public int CompanySize { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    public string? Info { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<Employer> Employers { get; set; } = new List<Employer>();

    [InverseProperty("Company")]
    public virtual ICollection<RecruitmentInfo> RecruitmentInfos { get; set; } = new List<RecruitmentInfo>();
}
