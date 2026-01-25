using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.DataAccess.Seed
{
    public static class DbSeeder
    {
        public static void Seed(WorkHubDbContext context)
        {
            SeedUsers(context);
            SeedCompanies(context);
            SeedEmployers(context);
            SeedSeekers(context);
            SeedRecruitments(context);
            SeedApplications(context);
        }

        // ================= USERS =================
        private static void SeedUsers(WorkHubDbContext context)
        {
            if (context.Users.Any()) return;

            context.Users.AddRange(
                new User
                {
                    Email = "admin@workhub.com",
                    Password = BCryptHelper.Encode("123456"),
                    Role = RoleMapper.MapRoleToRoleNumber(SD.Role_Admin),
                    IsVerified = true,
                    FullName = "Admin User",
                    Age = 30,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "employer@workhub.com",
                    Password = BCryptHelper.Encode("123456"),
                    Role = RoleMapper.MapRoleToRoleNumber(SD.Role_Employer),
                    IsVerified = true,
                    FullName = "Employer One",
                    Age = 35,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "seeker@workhub.com",
                    Password = BCryptHelper.Encode("123456"),
                    Role = RoleMapper.MapRoleToRoleNumber(SD.Role_JobSeeker),
                    IsVerified = true,
                    FullName = "Job Seeker",
                    Age = 24,
                    CreatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }

        // ================= COMPANY =================
        private static void SeedCompanies(WorkHubDbContext context)
        {
            if (context.Companies.Any()) return;

            context.Companies.Add(
                new Company
                {
                    CompanyName = "WorkHub Tech",
                    CompanyIndustry = "Software",
                    CompanySize = 50,
                    Location = "Ho Chi Minh City",
                    Phone = "0123456789",
                    Email = "contact@workhub.com",
                    Info = "A tech startup"
                }
            );

            context.SaveChanges();
        }

        // ================= EMPLOYER =================
        private static void SeedEmployers(WorkHubDbContext context)
        {
            if (context.Employers.Any()) return;

            var employerUser = context.Users.First(u => u.Role == 2);
            var company = context.Companies.First();

            context.Employers.Add(
                new Employer
                {
                    UserId = employerUser.Id,
                    CompanyId = company.Id,
                    Bio = "Hiring talented developers"
                }
            );

            context.SaveChanges();
        }

        // ================= SEEKER =================
        private static void SeedSeekers(WorkHubDbContext context)
        {
            if (context.Seekers.Any()) return;

            var seekerUser = context.Users.First(u => u.Role == 3);

            context.Seekers.Add(
                new Seeker
                {
                    UserId = seekerUser.Id,
                    Bio = "Looking for backend roles",
                    EducationLevel = "Bachelor",
                    Major = "Software Engineering",
                    Schedule = "Full-time"
                }
            );

            context.SaveChanges();
        }

        // ================= RECRUITMENT =================
        private static void SeedRecruitments(WorkHubDbContext context)
        {
            if (context.RecruitmentInfos.Any()) return;

            var employer = context.Employers.First();
            var company = context.Companies.First();

            context.RecruitmentInfos.Add(
                new RecruitmentInfo
                {
                    EmployerId = employer.Id,
                    CompanyId = company.Id,
                    JobName = "Junior .NET Developer",
                    JobType = "Full-time",
                    Location = "Remote",
                    Salary = "$800 - $1200",
                    CreatedAt = DateTime.UtcNow,
                    Status = true,
                    Schedule = "Mon-Fri"
                }
            );

            context.SaveChanges();
        }

        // ================= APPLICATION =================
        private static void SeedApplications(WorkHubDbContext context)
        {
            if (context.Applications.Any()) return;

            var seeker = context.Seekers.First();
            var job = context.RecruitmentInfos.First();

            context.Applications.Add(
                new Application
                {
                    SeekerId = seeker.Id,
                    RecruitmentId = job.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}
