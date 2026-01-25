using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service
{
    public class GoogleAuthService(IConfiguration config) : IGoogleAuthService
    {
        private readonly IConfiguration _config = config;



        public async Task<GoogleUserInfoDTO> VerifyTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                _config["Authentication:Google:ClientId"]!
            }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfoDTO
            {
                Email = payload.Email,
                Name = payload.Name,
                GoogleId = payload.Subject,
                PictureUrl = payload.Picture
            };
        }
    }
}
