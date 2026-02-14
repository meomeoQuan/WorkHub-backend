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
    public class UserFollowRepository : Repository<UserFollow>, IUserFollowRepository
    {
        private readonly WorkHubDbContext _db;
        public UserFollowRepository(WorkHubDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserFollow entity)
        {
            _db.UserFollows.Update(entity);
        }
    }

}
