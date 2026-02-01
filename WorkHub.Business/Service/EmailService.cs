using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Utility;

namespace WorkHub.Business.Service
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task SendEmailAsync(EmailRequestDTO emailRequestDTO)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                "WorkHub",
                _configuration["EmailSettings:Username"]
            ));

            email.To.Add(MailboxAddress.Parse(emailRequestDTO.To));
            email.Subject = emailRequestDTO.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = emailRequestDTO.Body
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }


        public async Task VerifyEmailAsync(VerifyEmailRequestDTO emailRequestDTO)
        {
            var user = await _unitOfWork.UserRepository
                .GetAsync(u => u.EmailVerificationToken == emailRequestDTO.Token);

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
