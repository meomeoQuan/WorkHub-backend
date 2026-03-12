using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service
{
    public class MediaService : IMediaService
    {
        private readonly Cloudinary _cloudinary;

        public MediaService(IOptions<CloudinaryOptions> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string?> UploadAsync(IFormFile file, string folder)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
               return null;
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string folder)
        {
            var uploadResult = new RawUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                return null;
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
    }
}
