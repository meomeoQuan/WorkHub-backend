using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class Application
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RecruitmentId { get; set; }

    public string? Status { get; set; }

    public string? CoverLetter { get; set; }

    public string? CvUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recruitment Recruitment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
