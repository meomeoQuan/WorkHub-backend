using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class PostRecruitment
{
    public int PostId { get; set; }
    public int RecruitmentId { get; set; }

    public virtual Post Post { get; set; } = null!;
    public virtual Recruitment Recruitment { get; set; } = null!;
}
