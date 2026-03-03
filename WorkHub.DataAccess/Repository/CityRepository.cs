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
    public class CityRepository : Repository<City>, ICityRepository
    {
        private readonly WorkHubDbContext _context;
        public CityRepository(WorkHubDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(City city)
        {
            _context.Cities.Update(city);
        }
    }
}
