using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Repository.IRepository
{
    public interface IUserSubscriptionRepository : IRepository<UserSubscription>
    {
        public void Update(UserSubscription userSubscription);
    }
}
