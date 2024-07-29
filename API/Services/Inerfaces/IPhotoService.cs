using CloudinaryDotNet.Actions;

namespace API.Services.Inerfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);

    }
}