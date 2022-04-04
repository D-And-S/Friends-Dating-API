using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Friends_Date_API.Services
{
    public class PhotoServices : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoServices(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsynch(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // Logic to upload file in cloudinary
                
                using var stream = file.OpenReadStream();
                var uplodParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                    // transformation means shape of photo
                    // weather our photo will circle or square 
                    Transformation = new Transformation()
                        .Height(500)
                        .Width(500)
                        .Crop("fill")
                        .Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uplodParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }
    }
}
