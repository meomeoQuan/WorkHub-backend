using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Repository
{
    public class RecruitmentInfoRepo : Repository<RecruitmentInfo>, IRecruitmentInfoRepo
    {
        private readonly WorkHubDbContext _context;
        public RecruitmentInfoRepo(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(RecruitmentInfo entity)
        {
            _context.RecruitmentInfos.Update(entity);
        }
    }
}
