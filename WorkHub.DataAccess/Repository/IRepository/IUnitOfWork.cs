using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.Models.Models;
using static Azure.Core.HttpHeader;

namespace WorkHub.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IRecruitmentInfoRepo RecruitmentInfoRepo { get; }
        public IUserRepository UserRepository { get; }

        public IPostRepository PostRepository { get; }

        public ICommentRepository CommentRepository { get; }

        public IPostLikeRepository PostLikeRepository { get; }

        public IUserFollowRepository userFollowRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public IUserSubscriptionRepository  UserSubscriptionRepository { get; }

       public IJobTypeRepository JobTypeRepo { get; }

        public ICategoryRepository JobCategoryRepo { get; }
        
        public IRepository<UserExperience> UserExperienceRepository { get; }
        public IRepository<UserEducation> UserEducationRepository { get; }
        public IRepository<UserSchedule> UserScheduleRepository { get; }

        public IRepository<Application> ApplicationRepository { get; }

        Task SaveAsync();
    }
}
