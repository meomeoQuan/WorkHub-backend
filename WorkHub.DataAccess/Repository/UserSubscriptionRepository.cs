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
    public class UserSubscriptionRepository : Repository<UserSubscription>, IUserSubscriptionRepository
    {
        private readonly WorkHubDbContext _context;
        public UserSubscriptionRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(UserSubscription userSubscription)
        {
            _context.UserSubscriptions.Update(userSubscription);
        }
    }
}
