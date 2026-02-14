using System;
using System.Collections.Generic;

namespace WorkHub.Models.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; }

    public int ? Role { get; set; }


    public bool? IsVerified { get; set; }

    public string? Provider { get; set; }

    public string? ProviderId { get; set; }

    public string? EmailVerificationToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<CommentLikes> CommentLikes { get; set; } = new List<CommentLikes>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();

    public virtual UserDetail? UserDetail { get; set; }

    public virtual ICollection<UserFollow> UserFollowFollowers { get; set; } = new List<UserFollow>();

    public virtual ICollection<UserFollow> UserFollowFollowings { get; set; } = new List<UserFollow>();

    public virtual ICollection<UserSchedule> UserSchedules { get; set; } = new List<UserSchedule>();
}
