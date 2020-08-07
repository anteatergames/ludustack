using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuduStack.Application.Services
{
    public abstract class BaseAppService : IDisposable
    {
        protected readonly IMapper mapper;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ICacheService cacheService;

        protected BaseAppService(IMapper mapper
            , IUnitOfWork unitOfWork
            , ICacheService cacheService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        protected BaseAppService(IBaseAppServiceCommon baseAppServiceCommon)
        {
            mapper = baseAppServiceCommon.Mapper;
            unitOfWork = baseAppServiceCommon.UnitOfWork;
            cacheService = baseAppServiceCommon.CacheService;
        }

        protected MediaType GetMediaType(string featuredImage)
        {
            if (string.IsNullOrWhiteSpace(featuredImage))
            {
                return MediaType.None;
            }

            string youtubePattern = @"(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+";

            Match match = Regex.Match(featuredImage, youtubePattern);

            if (match.Success)
            {
                return MediaType.Youtube;
            }

            string[] imageExtensions = new string[] { "jpg", "png", "gif", "tiff", "webp", "svg", "jfif", "jpeg", "bmp" };
            string[] videoExtensions = new string[] { "mp4", "avi", "mpeg", "vob", "webm", "mpg", "m4v", "wmv", "asf", "mov", "mpe", "3gp" };

            string extension = featuredImage.Split('.').Last();

            if (imageExtensions.Contains(extension.ToLower()))
            {
                return MediaType.Image;
            }
            else if (videoExtensions.Contains(extension.ToLower()))
            {
                return MediaType.Video;
            }

            return MediaType.Image;
        }

        protected static void SetBasePermissions(Guid currentUserId, IBaseViewModel vm)
        {
            vm.Permissions.CanEdit = vm.UserId == currentUserId;
            vm.Permissions.CanDelete = vm.UserId == currentUserId;
        }

        protected virtual void Dispose(bool dispose)
        {
            // dispose resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}