using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WorkHub.Business.Service.IService
{
    public interface IMediaService
    {
        Task<string?> UploadAsync(IFormFile file, string folder);
        Task<string?> UploadFileAsync(IFormFile file, string folder);
        Task<bool> DeleteAsync(string publicId);
    }
}
