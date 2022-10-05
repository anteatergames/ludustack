using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System.IO;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class CloudinaryService : IImageStorageService
    {
        public async Task<UploadResultVo> StoreMediaAsync(string container, string fileName, string extension, byte[] image, params string[] tags)
        {
            Cloudinary cloudinary = new Cloudinary(ConfigHelper.ConfigOptions.CloudinaryUrl);

            string fileNameFull = string.Format("{0}.{1}", fileName, extension);

            MediaType mediaType = ContentHelper.GetMediaType(fileNameFull);

            string publicId = string.Format("{0}/{1}", container, fileName);

            MemoryStream stream = new MemoryStream(image);

            if (mediaType == MediaType.Image)
            {
                ImageUploadParams uploadParams = new ImageUploadParams()
                {
                    PublicId = publicId,
                    File = new FileDescription(fileNameFull, stream),
                    Invalidate = true,
                };

                if (tags != null && tags.Length > 0)
                {
                    uploadParams.Tags = string.Join(',', tags).ToLower();
                }

                await cloudinary.UploadAsync(uploadParams);

                return new UploadResultVo(true, mediaType, fileNameFull, "Image uploaded");
            }
            else if (mediaType == MediaType.Video)
            {
                VideoUploadParams uploadParams = new VideoUploadParams
                {
                    PublicId = publicId,
                    File = new FileDescription(fileNameFull, stream),
                    Invalidate = true,
                };

                if (tags != null && tags.Length > 0)
                {
                    uploadParams.Tags = string.Join(',', tags).ToLower();
                }

                VideoUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

                return new UploadResultVo(true, mediaType, fileNameFull, "Video uploaded");
            }

            return new UploadResultVo(true, mediaType, null, "File not supported by LuduStack!");
        }

        public async Task<string> DeleteImageAsync(string container, string fileName)
        {
            Cloudinary cloudinary = new Cloudinary(ConfigHelper.ConfigOptions.CloudinaryUrl);

            string publicId = string.Format("{0}/{1}", container, fileName);

            await cloudinary.DeleteResourcesAsync(publicId);

            return fileName;
        }
    }
}