using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("Seeker")]
public partial class Seeker
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    [StringLength(255)]
    public string? CvUrl { get; set; }

    [StringLength(50)]
    public string? EducationLevel { get; set; }

    [StringLength(50)]
    public string? Major { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Schedule { get; set; }

    [InverseProperty("Seeker")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("UserId")]
    [InverseProperty("Seekers")]
    public virtual User User { get; set; } = null!;
}
