using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LuduStack.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class CloudinaryService : IImageStorageService
    {
        public CloudinaryService()
        {
        }

        public async Task<string> StoreImageAsync(string container, string fileName, byte[] image, params string[] tags)
        {
            Cloudinary cloudinary = new Cloudinary();

            fileName = fileName.Contains(".") ? fileName.Split(".")[0] : fileName;

            string publicId = String.Format("{0}/{1}", container, fileName);

            MemoryStream stream = new MemoryStream(image);

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                PublicId = publicId,
                File = new FileDescription(fileName, stream),
                Invalidate = true
            };

            if (tags != null && tags.Length > 0)
            {
                uploadParams.Tags = string.Join(',', tags).ToLower();
            }

            await cloudinary.UploadAsync(uploadParams);

            return fileName;
        }

        public async Task<string> DeleteImageAsync(string container, string fileName)
        {
            Cloudinary cloudinary = new Cloudinary();

            string publicId = String.Format("{0}/{1}", container, fileName);

            await cloudinary.DeleteResourcesAsync(publicId);

            return fileName;
        }
    }
}