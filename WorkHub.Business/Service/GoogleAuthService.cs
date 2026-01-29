using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service
{
    public class GoogleAuthService(IConfiguration config) : IGoogleAuthService
    {
        private readonly IConfiguration _config = config;
        private readonly HttpClient _httpClient = new();

     

        public async Task<GoogleUserInfoDTO> VerifyAuthCodeAsync(string authCode)
        {
            // 1️⃣ Exchange auth code → tokens
            var tokenResponse = await _httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                { "code", authCode },
                { "client_id", _config["Authentication:Google:ClientId"]!.Trim() },
                { "client_secret", _config["Authentication:Google:ClientSecret"]!.Trim() },
                { "redirect_uri", "postmessage" }, // 🔥 REQUIRED for SPA
                { "grant_type", "authorization_code" }
                })
            );

            var error = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Google token error: {error}");
            }


            var json = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(json)!;

            // 2️⃣ Validate id_token (reuse Google library)
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                tokenData.id_token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Authentication:Google:ClientId"]! }
                }
            );

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
