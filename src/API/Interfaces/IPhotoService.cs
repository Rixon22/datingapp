using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile formFile);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}