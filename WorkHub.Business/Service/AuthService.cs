using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;


namespace WorkHub.Business.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService, IGoogleAuthService googleAuthService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
        }

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            var existing = await _unitOfWork.UserRepository.GetAsync(c => c.Email == request.Email);
            if (existing != null)
                throw new Exception("Email already exists");

            var hash = BCryptHelper.Encode(request.Password);

            var user = new User
            {
                Email = request.Email,
                FullName = request.Email,
                Password = hash,
                Role = request.role,
            };

             _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<string> LoginAsync(LoginRequestDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(c => c.Email == request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");


            if (!BCryptHelper.Decode(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            return _jwtService.GenerateToken(user);
        }

        public async Task<string> GoogleLoginAsync(string idToken)
        {
            var googleUser = await _googleAuthService.VerifyTokenAsync(idToken);

            var user = await _unitOfWork.UserRepository
                .GetAsync(u => u.Email == googleUser.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = googleUser.Email,
                    FullName = googleUser.Name,
                    Role = RoleMapper.MapRoleToRoleNumber(SD.Role_JobSeeker),
                    Provider = SD.Provider_Google,
                    ProviderId = googleUser.GoogleId
                };

                _unitOfWork.UserRepository.Add(user);
                await _unitOfWork.SaveAsync();
            }

            return _jwtService.GenerateToken(user);
        }

    }

}
