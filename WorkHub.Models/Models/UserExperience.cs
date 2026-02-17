using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkHub.Models.Models;

public partial class UserExperience
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Company { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
