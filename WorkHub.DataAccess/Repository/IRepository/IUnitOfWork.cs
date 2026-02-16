using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
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
        Task SaveAsync();
    }
}
