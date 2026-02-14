using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();
    public virtual ICollection<CommentLikes> CommentLikes { get; set; } = new List<CommentLikes>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
