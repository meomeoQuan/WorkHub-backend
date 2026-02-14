using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
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
        }

        public IRecruitmentInfoRepo RecruitmentInfoRepo { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IPostRepository PostRepository { get; private set; }

        public ICommentRepository CommentRepository { get; private set; }

        public IPostLikeRepository PostLikeRepository { get; private set; }

        public IUserFollowRepository userFollowRepository { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
