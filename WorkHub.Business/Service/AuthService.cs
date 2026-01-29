using AutoMapper;
using Azure.Core;
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
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService, IGoogleAuthService googleAuthService,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
            _mapper = mapper;
        }

        public async Task<UserDTO?> RegisterAsync(RegisterRequestDTO request)
        {
            var existing = await _unitOfWork.UserRepository.GetAsync(c => c.Email.ToLower() == request.Email.ToLower());
            if (existing != null)
                throw new Exception("Email already exists");

            var hash = BCryptHelper.Encode(request.Password);

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Password = hash,
                Role = request.role,
                IsVerified = true,
            };

             _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(c => c.Email.ToLower() == request.Email.ToLower());
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if(user.IsVerified == false)
            {
                throw new UnauthorizedAccessException("Account has not been verified");
            }

            if (!BCryptHelper.Decode(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            var JwtToken = _jwtService.GenerateToken(user);

            return new LoginResponseDTO
            {
                Token = JwtToken,
                UserDTO = _mapper.Map<UserDTO>(user)
            };
        }

        public async Task<LoginResponseDTO?> GoogleLoginAsync(string authCode)
        {
            // 1️⃣ Verify auth code with Google (exchange + validate)
            var googleUser = await _googleAuthService.VerifyAuthCodeAsync(authCode);

            // 2️⃣ Find user by email
            var user = await _unitOfWork.UserRepository
                .GetAsync(u => u.Email.ToLower() == googleUser.Email.ToLower());

            // 3️⃣ Auto-register if not exists
            if (user == null)
            {
                user = new User
                {
                    Email = googleUser.Email,
                    FullName = googleUser.Name,
                    Role = RoleMapper.MapRoleToRoleNumber(SD.Role_JobSeeker),
                    Provider = SD.Provider_Google,
                    ProviderId = googleUser.GoogleId,
                    IsVerified = true
                };

                _unitOfWork.UserRepository.Add(user);
                await _unitOfWork.SaveAsync();
            }

            // 4️⃣ Generate JWT
            var jwtToken = _jwtService.GenerateToken(user);

            // 5️⃣ Return response
            return new LoginResponseDTO
            {
                Token = jwtToken,
                UserDTO = _mapper.Map<UserDTO>(user)
            };
        }


    }

}
