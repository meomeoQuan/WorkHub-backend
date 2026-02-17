using WorkHub.DataAccess.Data;
using WorkHub.Models.Models;
using WorkHub.Utility;

public static class DbSeeder
{
    public static void Seed(WorkHubDbContext context)
    {
        // ================= JOB TYPES =================
        if (!context.JobTypes.Any())
        {
            var fullTime = new JobType { Name = SD.JobType_FullTime };
            var partTime = new JobType { Name = SD.JobType_PartTime };
            var freelance = new JobType { Name = SD.JobType_Freelance };
            var seasonal = new JobType { Name = SD.JobType_Seasonal };

            context.JobTypes.AddRange(fullTime, partTime, freelance, seasonal);
            context.SaveChanges();
        }

        // ================= CATEGORIES =================
        if (!context.Categories.Any())
        {
            var catIT = new Category { Name = SD.Category_IT };
            var catRetail = new Category { Name = SD.Category_Retail };
            var catEdu = new Category { Name = SD.Category_Education };
            var catFB = new Category { Name = "Food & Beverage" };

            context.Categories.AddRange(catIT, catRetail, catEdu, catFB);
            context.SaveChanges();
        }

        // Check if users exist for the remaining data
        if (context.Users.Any()) return;

        var now = DateTime.UtcNow;
        var tenDaysAgo = now.AddDays(-10);

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

        // ================= USER DETAIL =================

        context.UserDetails.AddRange(
            new UserDetail { UserId = admin.Id, FullName = "Admin", Age = 30, Rating = 5 },
            new UserDetail { UserId = user1.Id, FullName = "User One", Age = 25, Rating = 4.5 },
            new UserDetail { UserId = user2.Id, FullName = "User Two", Age = 24, Rating = 4.2 }
        );

        // Need to fetch JobTypes and Categories since they might have been seeded previously
        var fullTimeEntity = context.JobTypes.FirstOrDefault(j => j.Name == SD.JobType_FullTime);
        var partTimeEntity = context.JobTypes.FirstOrDefault(j => j.Name == SD.JobType_PartTime);
        var catITEntity = context.Categories.FirstOrDefault(c => c.Name == SD.Category_IT);

        // ================= POSTS =================

        var post1 = new Post { UserId = user1.Id, Content = "Welcome to WorkHub 🚀" };
        var post2 = new Post { UserId = user2.Id, Content = "Hiring devs ASAP" };

        context.Posts.AddRange(post1, post2);
        context.SaveChanges();

        // ================= RECRUITMENT =================

        var rec1 = new Recruitment
        {
            UserId = user1.Id,
            PostId = post1.Id,
            JobName = "Junior .NET",
            CategoryId = catITEntity?.Id ?? 1, // Fallback safe
            JobTypeId = fullTimeEntity?.Id ?? 1,
            Salary = "$800",
            Location = "Ha Noi",
            Status = "Open",
            ExperienceLevel = "Entry",
            WorkSetting = "Onsite",
            CompanySize = "Medium",
            Requirements = "C#, EF Core",
            WorkTime = "Mon-Fri"
        };

        var rec2 = new Recruitment
        {
            UserId = user2.Id,
            PostId = post2.Id,
            JobName = "Frontend Dev",
            CategoryId = catITEntity?.Id ?? 1,
            JobTypeId = partTimeEntity?.Id ?? 1,
            Salary = "$500",
            Location = "Sai Gon",
            Status = "Open",
            ExperienceLevel = "Mid",
            WorkSetting = "Remote",
            CompanySize = "Startup",
            Requirements = "React",
            WorkTime = "Flexible"
        };

        context.Recruitments.AddRange(rec1, rec2);
        context.SaveChanges();

        // ================= APPLICATION =================

        var app1 = new Application { UserId = user2.Id, RecruitmentId = rec1.Id };
        var app2 = new Application { UserId = user1.Id, RecruitmentId = rec2.Id };

        context.Applications.AddRange(app1, app2);

        // ================= COMMENTS =================

        var comment = new Comment
        {
            UserId = user2.Id,
            PostId = post1.Id,
            Content = "Looks fire 🔥"
        };

        context.Comments.Add(comment);
        context.SaveChanges();

        // ================= COMMENT LIKES =================

        context.CommentLikes.Add(new CommentLikes
        {
            UserId = user1.Id,
            CommentId = comment.Id
        });

        // ================= POST LIKE =================

        context.PostLikes.Add(new PostLike
        {
            UserId = user2.Id,
            PostId = post1.Id
        });

        // ================= FOLLOW =================

        context.UserFollows.Add(new UserFollow
        {
            FollowerId = user1.Id,
            FollowingId = user2.Id
        });

        // ================= SCHEDULE =================

        context.UserSchedules.Add(new UserSchedule
        {
            UserId = user2.Id,
            Title = "Interview",
            StartTime = now,
            EndTime = now.AddHours(2)
        });

        // ================= ORDERS =================

        var order1 = new Order
        {
            UserId = user1.Id,
            OrderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = 2000,
            Status = "Paid",
            PaidAt = now
        };

        context.Orders.Add(order1);

        // ================= SUBSCRIPTIONS =================

        context.UserSubscriptions.AddRange(
            new UserSubscription
            {
                UserId = user1.Id,
                StartAt = now.AddDays(-5),
                EndAt = now.AddDays(30),
                IsActive = true
            },
            new UserSubscription
            {
                UserId = user2.Id,
                StartAt = now.AddDays(-10),
                EndAt = now.AddDays(-1),
                IsActive = false
            }
        );

        context.SaveChanges();
    }
}
