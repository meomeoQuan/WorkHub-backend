using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class Post
{
    public int Id { get; set; }

    public int UserId { get; set; }


    public string? Content { get; set; }

    public string? PostImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();   // 👈 ADD THIS


    public virtual User User { get; set; } = null!;
}
