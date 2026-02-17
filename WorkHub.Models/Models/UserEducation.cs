using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkHub.Models.Models;

public partial class UserEducation
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public string School { get; set; } = null!;

    public string Degree { get; set; } = null!;

    public string FieldOfStudy { get; set; } = null!;

    public string StartYear { get; set; } = null!;

    public string? EndYear { get; set; }

    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
