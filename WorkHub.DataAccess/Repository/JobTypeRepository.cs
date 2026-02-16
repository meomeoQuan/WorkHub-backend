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
    public class JobTypeRepository : Repository<JobType>, IJobTypeRepository
    {
        private readonly WorkHubDbContext _context;
        public JobTypeRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(JobType jobtype)
        {
            _context.JobTypes.Update(jobtype);
        }
    }
}
