using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Models.Models;

[Table("User")]
[Index("Email", Name = "UQ__User__A9D1053491BE2FC7", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(12)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public double? Rating { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? AvatarUrl { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public int Age { get; set; }

    public string? Provider { get; set; }
    public string? ProviderId { get; set; }


    [InverseProperty("User")]
    public virtual ICollection<Employer> Employers { get; set; } = new List<Employer>();

    [InverseProperty("User")]
    public virtual ICollection<Seeker> Seekers { get; set; } = new List<Seeker>();
}
