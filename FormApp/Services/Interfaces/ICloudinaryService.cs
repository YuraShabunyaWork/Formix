namespace Formix.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadPhotoAsync(IFormFile filePhoto);
        Task DeletePhotoAsync(string url);
    }
}
