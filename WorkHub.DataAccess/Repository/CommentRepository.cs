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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly WorkHubDbContext _context;
        public CommentRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Comment entity)
        {
          
         _context.Comments.Update(entity);
     
        }
  
    }
}
