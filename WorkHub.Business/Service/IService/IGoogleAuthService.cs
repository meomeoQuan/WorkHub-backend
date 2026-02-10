using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs.AuthDTOs;

namespace WorkHub.Business.Service.IService
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfoDTO> VerifyAuthCodeAsync(string authCode);

    }
}
