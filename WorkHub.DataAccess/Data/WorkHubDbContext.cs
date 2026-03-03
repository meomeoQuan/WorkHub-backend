using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Data;

public partial class WorkHubDbContext : DbContext
{
    public WorkHubDbContext()
    {
    }

    public WorkHubDbContext(DbContextOptions<WorkHubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<Recruitment> Recruitments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }
    public virtual DbSet<CommentLikes> CommentLikes { get; set; }

    public virtual DbSet<UserExperience> UserExperiences { get; set; }
    public virtual DbSet<UserEducation> UserEducations { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }

    public virtual DbSet<UserSchedule> UserSchedules { get; set; }

    public virtual DbSet<JobType> JobTypes { get; set; }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<City> Cities { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC0709DB1FF3");

            entity.ToTable("Application");

            entity.HasIndex(e => e.UserId, "IX_Application_UserId");

            entity.HasIndex(e => new { e.UserId, e.RecruitmentId }, "UQ_Application").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("New");

            entity.HasOne(d => d.Recruitment).WithMany(p => p.Applications)
                .HasForeignKey(d => d.RecruitmentId)
                .HasConstraintName("FK__Applicati__Recru__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Applicati__UserI__5165187F");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC0788A5562F");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.PostId, "IX_Comment_PostId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comment__ParentC__47DBAE45");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Comment__PostId__45F365D3");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__46E78A0C");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC07971C2B27");

            entity.ToTable("Post");

            entity.HasIndex(e => e.UserId, "IX_Post_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Post__UserId__4222D4EF");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId });

            entity.ToTable("PostLike");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostLike_Post");

            entity.HasOne(d => d.User).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostLike_User");
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Recruitm__3214EC079D1054A4");

            entity.ToTable("Recruitment");

            entity.HasIndex(e => e.UserId, "IX_Recruitment_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())");

            entity.Property(e => e.JobName)
                .HasMaxLength(255);

            // entity.Property(e => e.JobType).IsRequired(); // Removed conflicting scalar config

            entity.Property(e => e.Location)
                .HasMaxLength(255);

            entity.Property(e => e.Salary)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            // ===== NEW PROPERTIES =====

            // entity.Property(e => e.Category).HasMaxLength(255); // Removed conflicting scalar config

            entity.Property(e => e.Requirements)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Benefits)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.WorkTime)
                .HasMaxLength(255);

            // KEEP user link BUT REMOVE cascade
            entity.HasOne(d => d.User)
                .WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // MAIN relationship (Post owns Recruitment)
            entity.HasOne(r => r.Post)
                .WithMany(p => p.Recruitments)
                .HasForeignKey(r => r.PostId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(r => r.Category)
      .WithMany(c => c.Recruitments)
      .HasForeignKey(r => r.CategoryId);

            entity.HasOne(r => r.JobType)
                  .WithMany(j => j.Recruitments)
                  .HasForeignKey(r => r.JobTypeId);

            entity.HasOne(r => r.City)
                  .WithMany()
                  .HasForeignKey(r => r.CityId);

        });



        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0775A0391A");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D3D212FF").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(255);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.ProviderId).HasMaxLength(255);
            entity.Property(e => e.Role)
            .HasDefaultValue(1);

        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDeta__3214EC073F2B3337");

            entity.ToTable("UserDetail");

            entity.HasIndex(e => e.UserId, "UQ__UserDeta__1788CC4D395EC6B9").IsUnique();

            entity.Property(e => e.EducationLevel).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.IndustryFocus).HasMaxLength(255);
            entity.Property(e => e.JobPreference).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Major).HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            entity.Property(e => e.GoogleMapsEmbedUrl).HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.User).WithOne(p => p.UserDetail)
                .HasForeignKey<UserDetail>(d => d.UserId)
                .HasConstraintName("FK__UserDetai__UserI__3E52440B");
        });

        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FollowingId }).HasName("PK__UserFoll__79CB03351636060D");

            entity.ToTable("UserFollow");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK__UserFollo__Follo__571DF1D5");

            entity.HasOne(d => d.Following).WithMany(p => p.UserFollowFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__Follo__5812160E");
        });

        modelBuilder.Entity<UserSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSche__3214EC072EF4856D");

            entity.ToTable("UserSchedule");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserSchedules)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserSched__UserI__619B8048");
        });

        modelBuilder.Entity<CommentLikes>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommentId });

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.User)
                .WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Comment)
                .WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(o => o.OrderCode).IsRequired();

            entity.Property(o => o.Amount).IsRequired();

            entity.Property(o => o.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.Property(o => o.CreatedAt)
           .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.StartAt).IsRequired();

            entity.Property(o => o.EndAt).IsRequired();

            entity.Property(s => s.IsActive)
                .HasDefaultValue(false);

            entity.Property(s => s.Plan)
                .HasMaxLength(20)
                .HasDefaultValue("free");

            entity.HasIndex(s => s.UserId)
         .IsUnique();   // 🔥 ensures ONE subscription per user

            entity.HasOne(s => s.User)
                  .WithOne(u => u.Subscription)
                  .HasForeignKey<UserSubscription>(s => s.UserId)
                  .OnDelete(DeleteBehavior.NoAction);


        });


        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.HasData(
                new City { Id = 1, Name = "Remote" },
                new City { Id = 2, Name = "An Giang" },
                new City { Id = 3, Name = "Ba Ria - Vung Tau" },
                new City { Id = 4, Name = "Bac Lieu" },
                new City { Id = 5, Name = "Bac Giang" },
                new City { Id = 6, Name = "Bac Kan" },
                new City { Id = 7, Name = "Bac Ninh" },
                new City { Id = 8, Name = "Ben Tre" },
                new City { Id = 9, Name = "Binh Duong" },
                new City { Id = 10, Name = "Binh Dinh" },
                new City { Id = 11, Name = "Binh Phuoc" },
                new City { Id = 12, Name = "Binh Thuan" },
                new City { Id = 13, Name = "Ca Mau" },
                new City { Id = 14, Name = "Cao Bang" },
                new City { Id = 15, Name = "Can Tho" },
                new City { Id = 16, Name = "Da Nang" },
                new City { Id = 17, Name = "Dak Lak" },
                new City { Id = 18, Name = "Dak Nong" },
                new City { Id = 19, Name = "Dien Bien" },
                new City { Id = 20, Name = "Dong Nai" },
                new City { Id = 21, Name = "Dong Thap" },
                new City { Id = 22, Name = "Gia Lai" },
                new City { Id = 23, Name = "Ha Giang" },
                new City { Id = 24, Name = "Ha Nam" },
                new City { Id = 25, Name = "Ha Noi" },
                new City { Id = 26, Name = "Ha Tinh" },
                new City { Id = 27, Name = "Hai Duong" },
                new City { Id = 28, Name = "Hai Phong" },
                new City { Id = 29, Name = "Hau Giang" },
                new City { Id = 30, Name = "Hoa Binh" },
                new City { Id = 31, Name = "Hung Yen" },
                new City { Id = 32, Name = "Khanh Hoa" },
                new City { Id = 33, Name = "Kien Giang" },
                new City { Id = 34, Name = "Kon Tum" },
                new City { Id = 35, Name = "Lai Chau" },
                new City { Id = 36, Name = "Lang Son" },
                new City { Id = 37, Name = "Lao Cai" },
                new City { Id = 38, Name = "Lam Dong" },
                new City { Id = 39, Name = "Long An" },
                new City { Id = 40, Name = "Nam Dinh" },
                new City { Id = 41, Name = "Nghe An" },
                new City { Id = 42, Name = "Ninh Binh" },
                new City { Id = 43, Name = "Ninh Thuan" },
                new City { Id = 44, Name = "Phu Tho" },
                new City { Id = 45, Name = "Phu Yen" },
                new City { Id = 46, Name = "Quang Binh" },
                new City { Id = 47, Name = "Quang Nam" },
                new City { Id = 48, Name = "Quang Ngai" },
                new City { Id = 49, Name = "Quang Ninh" },
                new City { Id = 50, Name = "Quang Tri" },
                new City { Id = 51, Name = "Soc Trang" },
                new City { Id = 52, Name = "Son La" },
                new City { Id = 53, Name = "Tay Ninh" },
                new City { Id = 54, Name = "Thai Binh" },
                new City { Id = 55, Name = "Thai Nguyen" },
                new City { Id = 56, Name = "Thanh Hoa" },
                new City { Id = 57, Name = "Thua Thien Hue" },
                new City { Id = 58, Name = "Tien Giang" },
                new City { Id = 59, Name = "Tra Vinh" },
                new City { Id = 60, Name = "Tuyen Quang" },
                new City { Id = 61, Name = "Vinh Long" },
                new City { Id = 62, Name = "Vinh Phuc" },
                new City { Id = 63, Name = "Yen Bai" },
                new City { Id = 64, Name = "Ho Chi Minh City" }
            );
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
