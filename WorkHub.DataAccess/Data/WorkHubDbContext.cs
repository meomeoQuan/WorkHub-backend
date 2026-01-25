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

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<RecruitmentInfo> RecruitmentInfos { get; set; }

    public virtual DbSet<Seeker> Seekers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=database-1.cl0ioa06q5yt.ap-southeast-1.rds.amazonaws.com;Database=WorkHub;User Id=admin;Password=YourPassword123!;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC077AFC05A6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Recruitment).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Recruitment");

            entity.HasOne(d => d.Seeker).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Seeker");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company__3214EC070F87BB5D");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employer__3214EC07E3EE2679");

            entity.HasOne(d => d.Company).WithMany(p => p.Employers).HasConstraintName("FK_Employer_Company");

            entity.HasOne(d => d.User).WithMany(p => p.Employers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employer_User");
        });

        modelBuilder.Entity<RecruitmentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recruitm__3214EC0755D60B5E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Company).WithMany(p => p.RecruitmentInfos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruitment_Company");

            entity.HasOne(d => d.Employer).WithMany(p => p.RecruitmentInfos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruitment_Employer");
        });

        modelBuilder.Entity<Seeker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seeker__3214EC072FF42672");

            entity.HasOne(d => d.User).WithMany(p => p.Seekers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seeker_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0794ADE483");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
