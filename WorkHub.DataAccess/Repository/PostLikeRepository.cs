using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Repository
{
    public class PostLikeRepository : Repository<PostLike>, IPostLikeRepository
    {
        private readonly WorkHubDbContext _db;
        public PostLikeRepository(WorkHubDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(PostLike entity)
        {
           _db.PostLikes.Update(entity);
        }
    }

}
