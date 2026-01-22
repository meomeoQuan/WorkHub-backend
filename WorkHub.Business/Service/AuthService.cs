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

        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task RegisterAsync(RegisterRequest request)
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
                Role = RoleMapper.MapRoleToRoleNumber(SD.Role_JobSeeker)
            };

             _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(c => c.Email == request.Email);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (!BCryptHelper.Decode(request.Password, user.Password))
                throw new UnauthorizedAccessException();

            return _jwtService.GenerateToken(user);
        }
    }

}
