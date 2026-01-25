using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service.IService
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfoDTO> VerifyTokenAsync(string idToken);
    }
}
