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
    public class UserDetailRepository : Repository<UserDetail>, IUserDetailRepository
    {
        private readonly WorkHubDbContext _db;

        public UserDetailRepository(WorkHubDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserDetail userDetail)
        {
            _db.UserDetails.Update(userDetail);
        }
    }
}
