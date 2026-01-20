using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;

namespace WorkHub.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkHubDbContext _db;

        public UnitOfWork(WorkHubDbContext db)
        {
            _db = db;
            //Product = new ProductRepository(_db);
        }

        //public IProductRepository Product { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
