using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("Employer")]
public partial class Employer
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? CompanyId { get; set; }

    public string? Bio { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("Employers")]
    public virtual Company? Company { get; set; }

    [InverseProperty("Employer")]
    public virtual ICollection<RecruitmentInfo> RecruitmentInfos { get; set; } = new List<RecruitmentInfo>();

    [ForeignKey("UserId")]
    [InverseProperty("Employers")]
    public virtual User User { get; set; } = null!;
}
