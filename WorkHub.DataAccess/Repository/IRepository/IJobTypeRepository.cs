using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Repository.IRepository
{
    public interface IJobTypeRepository : IRepository<JobType>
    {
        void Update (JobType jobtype);
    }
}
