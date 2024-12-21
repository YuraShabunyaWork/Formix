using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Net;
using Formix.Services.Interfaces;

namespace Formix.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadPhotoAsync(IFormFile filePhoto)
        {
            using (var stream = filePhoto.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(filePhoto.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
            }
            return "/AvaDef.png";
        }
        public async Task DeletePhotoAsync(string url)
        {
            var publicId = GetPublicIdFromUrl(url);
            if (string.IsNullOrEmpty(publicId))
            {
                return;
            }
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
        }
        private string GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var fileName = Path.GetFileNameWithoutExtension(segments[^1]);
            return fileName;
        }
    }
}
