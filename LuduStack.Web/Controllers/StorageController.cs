using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Route("storage")]
    public class StorageController : SecureBaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger logger;

        private readonly string datetimeFormat = "yyyyMMddHHmmssfff";

        public StorageController(IHttpContextAccessor httpContextAccessor
            , ILogger<StorageController> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        [ResponseCache(CacheProfileName = "Never")]
        [Route("userimage/{type:alpha}/{userId:guid}/{name?}")]
        public async Task<IActionResult> UserImage(ImageType type, Guid userId, string name, string v)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = userId.ToString();
            }

            if (type == ImageType.ProfileImage)
            {
                name = string.Format("{0}_Personal", userId);
            }

            return await GetImage(type, userId, name, v);
        }

        [ResponseCache(CacheProfileName = "Default")]
        [Route("image/{type:alpha}/{userId:guid}/{name}")]
        [Route("image/{name}")]
        [Route("image")]
        public async Task<IActionResult> Image(ImageType type, Guid userId, string name, string v)
        {
            return await GetImage(type, userId, name, v);
        }

        private async Task<IActionResult> GetImage(ImageType type, Guid userId, string name, string v)
        {
            string baseUrl = Constants.DefaultCdnPath;
            name = name.Replace(Constants.DefaultImagePath, string.Empty);

            try
            {
                string storageBasePath = FormatBasePath(type, userId, baseUrl);

                string url = UrlFormatter.CdnImageCommon(userId, name);

                if (!string.IsNullOrWhiteSpace(v))
                {
                    url += "?v=" + v;
                }

                byte[] data;

                using (HttpClient webClient = new HttpClient())
                {
                    data = await webClient.GetByteArrayAsync(url);
                }

                string etag = ETagGenerator.GetETag(httpContextAccessor.HttpContext.Request.Path.ToString(), data);
                if (httpContextAccessor.HttpContext.Request.Headers.Keys.Contains(HeaderNames.IfNoneMatch) && httpContextAccessor.HttpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString() == etag)
                {
                    return new StatusCodeResult(StatusCodes.Status304NotModified);
                }
                httpContextAccessor.HttpContext.Response.Headers.Add(HeaderNames.ETag, new[] { etag });

                return File(new MemoryStream(data), "image/jpeg");
            }
            catch (Exception ex)
            {
                string msg = $"Unable to save get the Image.";
                logger.Log(LogLevel.Error, ex, msg);

                return ReturnDefaultImage(type);
            }
        }

        #region Profile

        [HttpPost]
        [Route("uploadavatar")]
        public async Task<IActionResult> UploadProfileAvatar(IFormFile image, string currentImage, Guid userId)
        {
            try
            {
                string imageUrl = string.Empty;

                if (image != null && image.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(image);

                        string filename = userId + "_Personal";

                        UploadResultVo uploadResult = await base.UploadImage(userId, ImageType.ProfileImage, filename, extension, fileBytes, EnvName);

                        uploadResult.FileSize = image?.Length;
                        uploadResult.OldImage = currentImage;

                        return Json(uploadResult);
                    }
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        [HttpPost]
        [Route("uploadprofilecoverimage")]
        public async Task<IActionResult> UploadProfileCoverImage(IFormFile image, string currentImage, Guid userId, Guid profileId)
        {
            try
            {
                string imageUrl = string.Empty;

                if (image != null && image.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(image);

                        string filename = profileId.ToString();

                        UploadResultVo uploadResult = await base.UploadImage(userId, ImageType.ProfileCover, filename, extension, fileBytes, EnvName);

                        uploadResult.FileSize = image?.Length;
                        uploadResult.OldImage = currentImage;

                        return Json(uploadResult);
                    }
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        #endregion Profile

        #region Game

        [HttpPost]
        [Route("uploadgamethumbnail")]
        public async Task<IActionResult> UploadGameThumbnail(IFormFile image, Guid gameId, string currentImage, Guid userId)
        {
            return await UploadGameImage(image, ImageType.GameThumbnail, Constants.DefaultGameThumbnail, gameId, currentImage, userId);
        }

        [HttpPost]
        [Route("uploadgamecoverimage")]
        public async Task<IActionResult> UploadGameCoverImage(IFormFile image, Guid gameId, string currentImage, Guid userId)
        {
            return await UploadGameImage(image, ImageType.GameCover, Constants.DefaultGameCoverImage, gameId, currentImage, userId);
        }

        private async Task<IActionResult> UploadGameImage(IFormFile image, ImageType type, string defaultImage, Guid gameId, string currentImage, Guid? userId)
        {
            try
            {
                Guid destinationUserId = userId.HasValue && userId.Value != Guid.Empty ? userId.Value : CurrentUserId;

                if (image != null && image.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(image);

                        Random rand = new Random(Guid.NewGuid().ToString().GetHashCode());

                        string filename = string.Format("{0}-{1}", DateTime.Now.ToString(datetimeFormat), rand.Next().ToString());

                        UploadResultVo uploadResult = await base.UploadImage(destinationUserId, type, filename, extension, fileBytes, EnvName);

                        uploadResult.FileSize = image?.Length;
                        uploadResult.OldImage = currentImage;
                        uploadResult.RelatedId = gameId;

                        return Json(uploadResult);
                    }
                }

                if (!string.IsNullOrWhiteSpace(currentImage) && !currentImage.Equals(defaultImage))
                {
                    string currentParam = GetImageNameFromUrl(currentImage);

                    base.DeleteGameImage(destinationUserId, type, currentParam);
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        #endregion Game

        #region Content

        [HttpPost]
        [Route("uploadmedia")]
        public async Task<IActionResult> UploadMedia(IFormFile upload, Guid? userId, string tag)
        {
            try
            {
                if (upload != null && upload.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        upload.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(upload);

                        Random rand = new Random(Guid.NewGuid().ToString().GetHashCode());

                        string filename = string.Format("{0}-{1}", DateTime.Now.ToString(datetimeFormat), rand.Next().ToString());

                        Guid destinationUserId = userId.HasValue && userId.Value != Guid.Empty ? userId.Value : CurrentUserId;

                        UploadResultVo uploadResult = await base.UploadContentMedia(destinationUserId, filename, extension, fileBytes, EnvName, tag);

                        return Json(uploadResult);
                    }
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        [HttpPost]
        [Route("uploadarticleimage")]
        public async Task<IActionResult> UploadArticleImage(IFormFile upload)
        {
            try
            {
                if (upload != null && upload.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        upload.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(upload);

                        string filename = DateTime.Now.ToString(datetimeFormat);

                        UploadResultVo uploadResult = await base.UploadContentMedia(CurrentUserId, filename, extension, fileBytes, EnvName);

                        uploadResult.FileSize = upload.Length;
                        uploadResult.Url = UrlFormatter.Image(CurrentUserId, ImageType.ContentImage, uploadResult.Filename);

                        return Json(uploadResult);
                    }
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        [HttpPost]
        [Route("uploadfeaturedimage")]
        public async Task<IActionResult> UploadFeaturedImage(IFormFile featuredimage, Guid id, string currentImage, Guid userId)
        {
            try
            {
                if (featuredimage != null && featuredimage.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        featuredimage.CopyTo(ms);

                        byte[] fileBytes = ms.ToArray();

                        string extension = GetFileExtension(featuredimage);

                        string filename = DateTime.Now.ToString(datetimeFormat);

                        if (userId == Guid.Empty)
                        {
                            userId = CurrentUserId;
                        }

                        UploadResultVo uploadResult = await base.UploadFeaturedImage(userId, filename, extension, fileBytes, EnvName);

                        uploadResult.FileSize = featuredimage?.Length;
                        uploadResult.RelatedId = id;
                        uploadResult.Url = UrlFormatter.Image(CurrentUserId, ImageType.ContentImage, uploadResult.Filename);

                        return Json(uploadResult);
                    }
                }

                if (!string.IsNullOrWhiteSpace(currentImage) && !currentImage.Equals(Constants.DefaultFeaturedImage))
                {
                    string currentParam = GetImageNameFromUrl(currentImage);

                    base.DeleteFeaturedImage(userId, currentParam);
                }

                return Json(new UploadResultVo("No file to upload"));
            }
            catch (Exception ex)
            {
                return Json(new UploadResultVo(ex.Message));
            }
        }

        #endregion Content

        #region Private Methods

        private IActionResult ReturnDefaultImage(ImageType type)
        {
            string defaultImageNotRooted = UrlFormatter.GetDefaultImage(type);

            string retorno = Path.Combine(HostEnvironment.WebRootPath, defaultImageNotRooted);

            byte[] bytes = System.IO.File.ReadAllBytes(retorno);

            return File(new MemoryStream(bytes), "image/png");
        }

        private static string GetImageNameFromUrl(string currentImage)
        {
            string[] urlSplit = currentImage.Split('/');
            string split = urlSplit[urlSplit.Length - 1];

            return split;
        }

        private static string FormatBasePath(ImageType type, Guid userId, string baseUrl)
        {
            string storageBasePath = string.Empty;

            switch (type)
            {
                case ImageType.ProfileImage:
                    storageBasePath = baseUrl + userId + "/" + type.ToString().ToLower() + "_";
                    break;

                case ImageType.ProfileCover:
                    storageBasePath = baseUrl + userId + "/" + type.ToString().ToLower() + "_";
                    break;

                default:
                    storageBasePath = baseUrl + userId + "/";
                    break;
            }

            return storageBasePath;
        }

        #endregion Private Methods
    }
}