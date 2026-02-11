using WorkHub.DataAccess.Data;
using WorkHub.Models.Models;
using WorkHub.Utility;

public static class DbSeeder
{
    public static void Seed(WorkHubDbContext context)
    {
        if (context.Users.Any()) return;

        // ================= USERS =================

        var users = new List<User>
        {
            new User
            {
                Email = "admin@gmail.com",
                FullName = "Admin",
                PasswordHash = BCryptHelper.Encode("123"),
                Role = RoleMapper.MapRoleToRoleNumber(SD.Role_Admin),
                IsVerified = true
            },
            new User
            {
                Email = "user1@gmail.com",
                FullName = "User One",
                PasswordHash = BCryptHelper.Encode("123"),
                Role =  RoleMapper.MapRoleToRoleNumber(SD.Role_User),
                IsVerified = true
            },
            new User
            {
                Email = "user2@gmail.com",
                FullName = "User Two",
                PasswordHash = BCryptHelper.Encode("123"),
                Role = RoleMapper.MapRoleToRoleNumber(SD.Role_User),
                IsVerified = true
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges(); // USERS FIRST

        // ================= USER DETAILS =================

        context.UserDetails.AddRange(users.Select(u => new UserDetail
        {
            UserId = u.Id,
            FullName = u.Email,
            Age = 25
        }));

        // ================= RECRUITMENTS =================

        var admin = users.First(x => x.Role == RoleMapper.MapRoleToRoleNumber(SD.Role_Admin));

        var recruitments = new List<Recruitment>
        {
            new Recruitment { UserId = admin.Id, JobName = "Junior .NET", JobType = "Full-time", Salary = "$800", Status = "Open" },
            new Recruitment { UserId = admin.Id, JobName = "Frontend Intern", JobType = "Intern", Salary = "$500", Status = "Open" }
        };

        context.Recruitments.AddRange(recruitments);
        context.SaveChanges(); // 🔥 IMPORTANT — now Recruitment IDs exist

        // ================= APPLICATIONS =================

        context.Applications.AddRange(
            new Application { UserId = users[1].Id, RecruitmentId = recruitments[0].Id },
            new Application { UserId = users[2].Id, RecruitmentId = recruitments[0].Id }
        );

        // ================= POSTS =================

        var posts = new List<Post>
        {
            new Post { UserId = admin.Id, Content = "Hello WorkHub" },
            new Post { UserId = admin.Id, Content = "Hiring devs" }
        };

        context.Posts.AddRange(posts);

        // ================= COMMENTS =================

        context.Comments.Add(new Comment
        {
            UserId = users[2].Id,
            Post = posts[0],
            Content = "Nice post"
        });

        // ================= FOLLOW =================

        context.UserFollows.Add(new UserFollow
        {
            FollowerId = users[1].Id,
            FollowingId = users[0].Id
        });

        // ================= LIKE =================

        context.PostLikes.Add(new PostLike
        {
            UserId = users[2].Id,
            Post = posts[0]
        });

        // ================= SCHEDULE =================

        context.UserSchedules.Add(new UserSchedule
        {
            UserId = users[2].Id,
            Title = "Interview",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        });

        // FINAL SAVE
        context.SaveChanges();
    }
}
