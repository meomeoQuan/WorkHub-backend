using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task SendEmailAsync(EmailRequestDTO emailRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task VerifyEmailAsync(string token)
        {
            var user = await _unitOfWork.UserRepository
                .GetAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                throw new Exception("Invalid token");

            // 🔥 EXPIRY CHECK HAPPENS HERE
            if (user.TokenExpiry < DateTime.UtcNow)
                throw new Exception("Token has expired");

            user.IsVerified = true;
            user.EmailVerificationToken = null;
            user.TokenExpiry = null;

            await _unitOfWork.SaveAsync();
        }

    }
}
