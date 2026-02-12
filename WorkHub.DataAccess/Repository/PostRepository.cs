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
    public class PostRepository : Repository<Post>, IPostRepository
    {

        private readonly WorkHubDbContext _context;
        public PostRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Post entity)
        {
            _context.Posts.Update(entity);
        }
    }
}
