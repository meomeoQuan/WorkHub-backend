using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using static Azure.Core.HttpHeader;

namespace WorkHub.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IRecruitmentInfoRepo RecruitmentInfoRepo { get; }
        Task SaveAsync();
    }
}
