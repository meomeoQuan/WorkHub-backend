using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.Models.Models;
using WorkHub.DataAccess.Repository.IRepository;

namespace WorkHub.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkHubDbContext _db;

        public UnitOfWork(WorkHubDbContext db)
        {
            _db = db;
            RecruitmentInfoRepo = new RecruitmentInfoRepo(_db);
            UserRepository = new UserRepository(_db);
            PostRepository =  new PostRepository(_db);
            CommentRepository = new CommentRepository(_db);
            PostLikeRepository = new PostLikeRepository(_db);
            userFollowRepository = new UserFollowRepository(_db);
            OrderRepository = new OrderRepository(_db);
            UserSubscriptionRepository = new UserSubscriptionRepository(_db);
            JobCategoryRepo = new CategoryRepository(_db);
            JobTypeRepo = new JobTypeRepository(_db);

            UserExperienceRepository = new Repository<UserExperience>(_db);
            UserEducationRepository = new Repository<UserEducation>(_db);
            UserScheduleRepository = new Repository<UserSchedule>(_db);
        }

        public IRecruitmentInfoRepo RecruitmentInfoRepo { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IPostRepository PostRepository { get; private set; }

        public ICommentRepository CommentRepository { get; private set; }

        public IPostLikeRepository PostLikeRepository { get; private set; }

        public IUserFollowRepository userFollowRepository { get; private set; }

        public IOrderRepository OrderRepository { get; private set; }

        public IUserSubscriptionRepository UserSubscriptionRepository { get; private set; }

        public IJobTypeRepository JobTypeRepo { get; private set; }

        public ICategoryRepository JobCategoryRepo { get; private set; }

        public IRepository<UserExperience> UserExperienceRepository { get; private set; }
        public IRepository<UserEducation> UserEducationRepository { get; private set; }
        public IRepository<UserSchedule> UserScheduleRepository { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
