using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class UserDetail
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public string? Location { get; set; }

    public int? Age { get; set; }

    public double? Rating { get; set; }

    public string? EducationLevel { get; set; }

    public string? Major { get; set; }

    public string? CvUrl { get; set; }

    public string? JobPreference { get; set; }

    public string? Skills { get; set; }

    public string? IndustryFocus { get; set; }

    public string? Website { get; set; }

    public string? CompanySize { get; set; }

    public int? FoundedYear { get; set; }

    public string? Description { get; set; }
    public virtual User User { get; set; } = null!;
}
