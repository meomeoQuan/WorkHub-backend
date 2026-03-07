using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.Models;

namespace WorkHub.DataAccess.Repository
{
    public class PostRecruitmentRepository : Repository<PostRecruitment>, IPostRecruitmentRepository
    {
        private readonly WorkHubDbContext _context;

        public PostRecruitmentRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
