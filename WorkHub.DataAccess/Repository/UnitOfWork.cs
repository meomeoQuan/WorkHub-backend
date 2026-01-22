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
            RecruitmentInfoRepo = new RecruitmentInfoRepo(_db);
            UserRepository = new UserRepository(_db);
        }

        public IRecruitmentInfoRepo RecruitmentInfoRepo { get; private set; }

        public IUserRepository UserRepository { get; private set; }



        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
