using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("Application")]
[Index("SeekerId", "RecruitmentId", Name = "UQ_Application", IsUnique = true)]
public partial class Application
{
    [Key]
    public int Id { get; set; }

    public int SeekerId { get; set; }

    public int RecruitmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("RecruitmentId")]
    [InverseProperty("Applications")]
    public virtual RecruitmentInfo Recruitment { get; set; } = null!;

    [ForeignKey("SeekerId")]
    [InverseProperty("Applications")]
    public virtual Seeker Seeker { get; set; } = null!;
}
