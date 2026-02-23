using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs;
using WorkHub.Models.DTOs.ModelDTOs.PaymentDTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs;
using WorkHub.Models.DTOs.ModelDTOs.MyApplicationDTOs;
using WorkHub.Models.DTOs.ModelDTOs.ScheduleDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 🔥 THIS IS WHAT YOU MISSED
            CreateMap<Recruitment, RecruitmentOverviewInfoDTO>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.User.UserDetail.Rating))
                .ForMember(d => d.JobType, o => o.MapFrom(s => s.JobType.Name))
                .ForMember(d => d.description, o => o.MapFrom(s => s.Post.Content))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Avatar, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl));

            CreateMap<Recruitment, RecruitmentDetailInfoDTO>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.JobType, o => o.MapFrom(s => s.JobType.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Post.Content))
                .ForMember(d => d.Schedule, o => o.MapFrom(s => s.WorkTime))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status == "Active"))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Avatar, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl))
                .ForMember(d => d.Requirements, o => o.MapFrom(s => s.Requirements))
                .ForMember(d => d.Benefits, o => o.MapFrom(s => s.Benefits))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.CompanyBio, o => o.MapFrom(s => s.User.UserDetail.Bio))
                .ForMember(d => d.CompanyDescription, o => o.MapFrom(s => s.User.UserDetail.Description))
                .ForMember(d => d.CompanyLocation, o => o.MapFrom(s => s.User.UserDetail.Location))
                .ForMember(d => d.CompanyRating, o => o.MapFrom(s => s.User.UserDetail.Rating))
                .ForMember(d => d.CompanyIndustry, o => o.MapFrom(s => s.User.UserDetail.IndustryFocus));

            CreateMap<Recruitment, RecruitmentSelectDTO>()
             .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.UserAvatar, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl));

            CreateMap<CreateJobRequestDTO, Recruitment>()
                .ForMember(d => d.JobName, o => o.MapFrom(s => s.JobTitle))
                .ForMember(d => d.Salary, o => o.MapFrom(s => s.SalaryRange))
                .ForMember(d => d.Location, o => o.MapFrom(s => s.Location))
                .ForMember(d => d.CategoryId, o => o.Ignore()) // Handle manually in Controller
                .ForMember(d => d.Category, o => o.Ignore()) // Ignore Nav Prop
                .ForMember(d => d.Requirements, o => o.MapFrom(s => s.Requirements))
                .ForMember(d => d.Benefits, o => o.MapFrom(s => s.Benefits))
                .ForMember(d => d.WorkTime, o => o.MapFrom(s => s.WorkTime))
                .ForMember(d => d.JobTypeId, o => o.MapFrom(s => s.JobType)) // Map to FK
                .ForMember(d => d.JobType, o => o.Ignore()) // Ignore Nav Prop
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.PostId, o => o.Ignore());



            //=================== Auth MAPPING =================
            CreateMap<User, UserDTO>()
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.AvatarUrl : null))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.PaymentPlan, o => o.MapFrom(s => s.Subscription != null ? s.Subscription.Plan : "free"));

            //=================== Home MAPPING =================

            CreateMap<User, UserFeatureDTO>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.ratingCount, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Rating : 0))
            .ForMember(d => d.ActiveJob, o => o.MapFrom(s => s.Recruitments.Count));


            //=================== Jobs Page MAPPING =================

            CreateMap<Recruitment, JobDTO>()
             .ForMember(d => d.JobType, o => o.MapFrom(s => s.JobType.Name))
             .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name));

            CreateMap<Post, JobPostDTO>()
                .ForMember(d => d.PostId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.User.UserDetail.Rating))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl))
                .ForMember(d => d.PostImage, o => o.MapFrom(s => s.PostImageUrl))
                .ForMember(d => d.LikeCount, o => o.MapFrom(s => s.PostLikes.Count))
                .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Comments.Count))
                .ForMember(d => d.Jobs, o => o.MapFrom(s => s.Recruitments));



            CreateMap<Comment, CommentDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ParentCommentId, o => o.MapFrom(s => s.ParentCommentId))
            .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.User.Id))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.UserUrl, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl))
            .ForMember(d => d.PostId, o => o.MapFrom(s => s.PostId));

            CreateMap<CreatePaymentDTO, Order>()
            .ForMember(d => d.Amount, o => o.MapFrom(s => s.TotalAmount))
            .ForMember(d => d.Status, o => o.MapFrom(_ => "Pending"))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.OrderCode, o => o.Ignore())
            .ForMember(d => d.PaidAt, o => o.Ignore());

            CreateMap<JobType, JobTypeDTO>();
            CreateMap<Category, CategoryDTO>();

            //=================== User Profile MAPPING =================
            CreateMap<UserExperience, UserExperienceDTO>().ReverseMap();
            CreateMap<UserEducation, UserEducationDTO>().ReverseMap();
            CreateMap<UserSchedule, UserScheduleDTO>().ReverseMap();

            CreateMap<User, UserProfileDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role))
                .ForMember(d => d.Provider, o => o.MapFrom(s => s.Provider))
                // Map from UserDetail
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.AvatarUrl : null))
                .ForMember(d => d.Location, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Location : null))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Bio : null))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.JobPreference : null))
                .ForMember(d => d.About, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Bio : null))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Description : null))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Rating : 0))
                .ForMember(d => d.CvUrl, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.CvUrl : null))
                .ForMember(d => d.Website, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Website : null))
                // .ForMember(d => d.CompanySize, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.CompanySize : null)) // Keep if needed for profile but remove from recruitment context if necessary
                .ForMember(d => d.FoundedYear, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.FoundedYear : null))
                .ForMember(d => d.Industry, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.IndustryFocus : null))
                .ForMember(d => d.GoogleMapsEmbedUrl, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.GoogleMapsEmbedUrl : null))
                // Collections
                .ForMember(d => d.Experiences, o => o.MapFrom(s => s.UserExperiences))
                .ForMember(d => d.Educations, o => o.MapFrom(s => s.UserEducations))
                .ForMember(d => d.Schedules, o => o.MapFrom(s => s.UserSchedules))
                // Skills (Splitting string)
                .ForMember(d => d.Skills, o => o.MapFrom(s => s.UserDetail != null && !string.IsNullOrEmpty(s.UserDetail.Skills) 
                    ? s.UserDetail.Skills.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList() 
                    : new List<string>()));

            CreateMap<UserProfileDTO, User>()
                .ForMember(d => d.UserDetail, o => o.MapFrom(s => s))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Email, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.UserExperiences, o => o.Ignore())
                .ForMember(d => d.UserEducations, o => o.Ignore())
                .ForMember(d => d.UserSchedules, o => o.Ignore());

            CreateMap<UserProfileDTO, UserDetail>()
                 .ForMember(d => d.UserId, o => o.Ignore())
                 .ForMember(d => d.Id, o => o.Ignore())
                 .ForMember(d => d.JobPreference, o => o.MapFrom(s => s.Title))
                 .ForMember(d => d.IndustryFocus, o => o.MapFrom(s => s.Industry))
                 .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                 .ForMember(d => d.GoogleMapsEmbedUrl, o => o.MapFrom(s => s.GoogleMapsEmbedUrl))
                 .ForMember(d => d.Skills, o => o.MapFrom(s => string.Join(",", s.Skills)));






            CreateMap<Application, ApplicationDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ApplicantId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ApplicantEmail, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.ApplicantAvatar, o => o.MapFrom(s => s.User.UserDetail != null ? s.User.UserDetail.AvatarUrl : null))
                .ForMember(d => d.ApplicantLocation, o => o.MapFrom(s => s.User.UserDetail != null ? s.User.UserDetail.Location : null))
                .ForMember(d => d.ApplicantSchool, o => o.MapFrom(s => s.User.UserEducations != null && s.User.UserEducations.Any() ? s.User.UserEducations.FirstOrDefault().School : null))
                .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Recruitment.JobName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.AppliedDate, o => o.MapFrom(s => s.CreatedAt));

            CreateMap<Application, ApplicationDetailDTO>()
                // Candidate Info
                .ForMember(d => d.ApplicantId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ApplicantEmail, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.ApplicantPhone, o => o.MapFrom(s => s.User.Phone))
                .ForMember(d => d.ApplicantLocation, o => o.MapFrom(s => s.User.UserDetail != null ? s.User.UserDetail.Location : null))
                .ForMember(d => d.ApplicantAvatar, o => o.MapFrom(s => s.User.UserDetail != null ? s.User.UserDetail.AvatarUrl : null))
                // Application Info
                .ForMember(d => d.AppliedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.CoverLetter, o => o.MapFrom(s => s.CoverLetter))
                .ForMember(d => d.CvUrl, o => o.MapFrom(s => s.CvUrl))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                // Job Info
                .ForMember(d => d.RecruitmentId, o => o.MapFrom(s => s.RecruitmentId))
                .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Recruitment.JobName))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Recruitment.User.FullName)) // Assuming Recruiter Name is Company Name equivalent here
                .ForMember(d => d.JobLocation, o => o.MapFrom(s => s.Recruitment.Location))
                // Collections
                .ForMember(d => d.Educations, o => o.MapFrom(s => s.User.UserEducations))
                .ForMember(d => d.Experiences, o => o.MapFrom(s => s.User.UserExperiences));

            CreateMap<Recruitment, JobNameDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.JobName, o => o.MapFrom(s => s.JobName));

            //=================== My Application MAPPING =================

            CreateMap<Application, MyApplicationDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.JobId, o => o.MapFrom(s => s.RecruitmentId))
                .ForMember(d => d.JobName, o => o.MapFrom(s => s.Recruitment.JobName))
                .ForMember(d => d.Company, o => o.MapFrom(s => s.Recruitment.User.FullName))
                .ForMember(d => d.CompanyLogo, o => o.MapFrom(s => s.Recruitment.User.UserDetail != null ? s.Recruitment.User.UserDetail.AvatarUrl : null))
                .ForMember(d => d.Location, o => o.MapFrom(s => s.Recruitment.Location))
                 .ForMember(d => d.JobType, o => o.MapFrom(s => s.Recruitment.JobType.Name))
                .ForMember(d => d.Salary, o => o.MapFrom(s => s.Recruitment.Salary))
                .ForMember(d => d.AppliedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.Status, o => o.MapFrom(s =>
                    s.Status == ApplicationStatus.New ? "Pending" :
                    s.Status == ApplicationStatus.Reviewing ? "Under Review" :
                    s.Status)); // Fallback or Accepted/Rejected

            //=================== Schedule MAPPING =================
            CreateMap<CreateScheduleDTO, UserSchedule>();
            CreateMap<UpdateScheduleDTO, UserSchedule>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UserSchedule, ScheduleViewDTO>();
        }
    }
}
