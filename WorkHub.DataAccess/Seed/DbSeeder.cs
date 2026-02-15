using WorkHub.DataAccess.Data;
using WorkHub.Models.Enums;
using WorkHub.Models.Models;
using WorkHub.Utility;

public static class DbSeeder
{
    public static void Seed(WorkHubDbContext context)
    {
        var tenDaysAgo = DateTime.UtcNow.AddDays(-10);

        if (context.Users.Any()) return;

        // ================= USERS =================

        var admin = new User
        {
            Email = "admin@gmail.com",
            FullName = "Admin",
            PasswordHash = BCryptHelper.Encode("123"),
            Role = RoleMapper.MapRoleToRoleNumber(SD.Role_Admin),
            IsVerified = true,
            CreatedAt = tenDaysAgo
        };

        var user1 = new User
        {
            Email = "user1@gmail.com",
            FullName = "User One",
            PasswordHash = BCryptHelper.Encode("123"),
            Role = RoleMapper.MapRoleToRoleNumber(SD.Role_User),
            IsVerified = true,
            CreatedAt = tenDaysAgo
        };

        var user2 = new User
        {
            Email = "user2@gmail.com",
            FullName = "User Two",
            PasswordHash = BCryptHelper.Encode("123"),
            Role = RoleMapper.MapRoleToRoleNumber(SD.Role_User),
            IsVerified = true,
            CreatedAt = tenDaysAgo
        };

        context.Users.AddRange(admin, user1, user2);
        context.SaveChanges();

        // ================= USER DETAILS =================

        context.UserDetails.AddRange(
            new UserDetail { UserId = admin.Id, FullName = admin.FullName, Age = 30 },
            new UserDetail { UserId = user1.Id, FullName = user1.FullName, Age = 25 },
            new UserDetail { UserId = user2.Id, FullName = user2.FullName, Age = 24 }
        );

        // ================= POSTS (ONLY NORMAL USERS) =================

        var post1 = new Post
        {
            UserId = user1.Id,
            Content = "Welcome to WorkHub 🚀"
        };

        var post2 = new Post
        {
            UserId = user2.Id,
            Content = "Looking for frontend + backend devs"
        };

        context.Posts.AddRange(post1, post2);
        context.SaveChanges();

        // ================= RECRUITMENTS (ATTACHED TO POSTS) =================

        var recruitment1 = new Recruitment
        {
            UserId = user1.Id,
            PostId = post1.Id,
            JobName = "Junior .NET",
            JobType = JobType.FullTime,
            Salary = "$800",
            Location = "Ha noi",
            Category = "Tech & IT",
            ExperienceLevel = "Entry Level",
            WorkSetting = "On-site",
            CompanySize = "Medium",
            Status = "Open"
        };

        var recruitment2 = new Recruitment
        {
            UserId = user2.Id,
            PostId = post2.Id,
            JobName = "Frontend Developer",
            JobType = JobType.PartTime,
            Salary = "$500",
            Location = "sai gon",
            Category = "Tech & IT",
            ExperienceLevel = "Mid Level",
            WorkSetting = "Remote",
            CompanySize = "Startup",
            Status = "Open"
        };

        var recruitment3 = new Recruitment
        {
            UserId = user1.Id,
            PostId = post1.Id,
            JobName = "Barista",
            JobType = JobType.PartTime,
            Salary = "$15/hr",
            Location = "da nang",
            Category = "Food & Beverage",
            ExperienceLevel = "Entry Level",
            WorkSetting = "On-site",
            CompanySize = "Small",
            Status = "Open"
        };

        context.Recruitments.AddRange(recruitment1, recruitment2, recruitment3);
        context.SaveChanges();

        // ================= APPLICATIONS =================

        context.Applications.AddRange(
            new Application { UserId = user2.Id, RecruitmentId = recruitment1.Id },
            new Application { UserId = user1.Id, RecruitmentId = recruitment2.Id }
        );

        // ================= COMMENT =================

        context.Comments.Add(new Comment
        {
            UserId = user2.Id,
            PostId = post1.Id,
            Content = "Nice post!"
        });

        // ================= FOLLOW =================

        context.UserFollows.Add(new UserFollow
        {
            FollowerId = user1.Id,
            FollowingId = user2.Id
        });

        // ================= LIKE =================

        context.PostLikes.Add(new PostLike
        {
            UserId = user2.Id,
            PostId = post1.Id
        });

        // ================= SCHEDULE =================

        context.UserSchedules.Add(new UserSchedule
        {
            UserId = user2.Id,
            Title = "Interview",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        });

        context.SaveChanges();
    }

}
