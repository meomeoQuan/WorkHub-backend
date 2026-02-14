using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobsDTOs;
using WorkHub.Models.Models;

namespace WorkHub.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 🔥 THIS IS WHAT YOU MISSED
            CreateMap<Recruitment, RecruitmentOverviewInfoDTO>()
                .ForMember(d => d.description, o => o.MapFrom(s => s.Post.Content))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Avatar, o => o.MapFrom(s => s.User.UserDetail.AvatarUrl));

            //=================== Auth MAPPING =================
            CreateMap<User,UserDTO>();

            //=================== Home MAPPING =================

            CreateMap<User, UserFeatureDTO>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.ratingCount, o => o.MapFrom(s => s.UserDetail != null ? s.UserDetail.Rating : 0))
            .ForMember(d => d.ActiveJob, o => o.MapFrom(s => s.Recruitments.Count));


            //=================== Jobs Page MAPPING =================

            CreateMap<Recruitment, JobDTO>();

            CreateMap<Post, JobPostDTO>()
                .ForMember(d => d.PostId, o => o.MapFrom(s => s.Id))
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



        }
    }
}
