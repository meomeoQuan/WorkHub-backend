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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {

        private readonly WorkHubDbContext _context;
        public OrderRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
           _context.Orders.Update(order);
        }
    }
}
