using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsynch(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
