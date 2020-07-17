using CountryData;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.Services
{
    public abstract class ProfileBaseAppService : BaseAppService, IProfileBaseAppService
    {
        protected readonly IProfileDomainService profileDomainService;

        protected ProfileBaseAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon.Mapper, profileBaseAppServiceCommon.UnitOfWork, profileBaseAppServiceCommon.CacheService)
        {
            profileDomainService = profileBaseAppServiceCommon.ProfileDomainService;
        }

        #region Profile

        public void SetProfileCache(Guid userId, UserProfile value)
        {
            cacheService.Set<string, UserProfile>(FormatProfileCacheId(userId), value);
        }

        public void SetProfileCache(Guid userId, ProfileViewModel viewModel)
        {
            UserProfile model = mapper.Map<UserProfile>(viewModel);

            SetProfileCache(viewModel.UserId, model);
        }

        private UserProfile GetProfileFromCache(Guid userId)
        {
            UserProfile fromCache = cacheService.Get<string, UserProfile>(FormatProfileCacheId(userId));

            return fromCache;
        }

        protected UserProfile GetCachedProfileByUserId(Guid userId)
        {
            UserProfile profile = GetProfileFromCache(userId);

            if (profile == null)
            {
                UserProfile profileFromDb = profileDomainService.GetByUserId(userId).FirstOrDefault();

                if (profileFromDb != null)
                {
                    SetProfileCache(userId, profileFromDb);
                    profile = profileFromDb;
                }
            }

            return profile;
        }

        public ProfileViewModel GetUserProfileWithCache(Guid userId)
        {
            UserProfile model = GetProfileFromCache(userId);

            if (model == null)
            {
                model = profileDomainService.GetByUserId(userId).FirstOrDefault();
            }

            ProfileViewModel viewModel = mapper.Map<ProfileViewModel>(model);

            return viewModel;
        }

        #endregion Profile

        #region Generics

        private void SetOjectOnCache<T>(Guid id, T value, string preffix)
        {
            cacheService.Set<string, T>(FormatObjectCacheId(preffix, id), value);
        }

        private T GetObjectFromCache<T>(Guid id, string preffix) where T : Entity
        {
            T fromCache = cacheService.Get<string, T>(FormatObjectCacheId(preffix, id));

            return fromCache;
        }

        private T GetCachedObjectById<T>(IDomainService<T> domainService, Guid id, string preffix) where T : Entity
        {
            T obj = GetObjectFromCache<T>(id, preffix);

            if (obj == null)
            {
                T objectFromDb = domainService.GetById(id);

                if (objectFromDb != null)
                {
                    SetOjectOnCache(id, objectFromDb, preffix);
                    obj = objectFromDb;
                }
            }

            return obj;
        }

        public OperationResultVo GetCountries(Guid currentUserId)
        {
            try
            {
                IEnumerable<SelectListItemVo> countries = CountryLoader.CountryInfo.Select(x => new SelectListItemVo(x.Name, x.Name)).OrderBy(x => x.Text);

                return new OperationResultListVo<SelectListItemVo>(countries);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        protected static string SetFeaturedImage(Guid userId, string thumbnailUrl, ImageRenderType imageType, string defaultImage)
        {
            if (string.IsNullOrWhiteSpace(thumbnailUrl) || defaultImage.NoExtension().Contains(thumbnailUrl.NoExtension()))
            {
                return Constants.DefaultCourseThumbnail;
            }
            else
            {
                switch (imageType)
                {
                    case ImageRenderType.LowQuality:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, 278, 10);

                    case ImageRenderType.Responsive:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, 278);
                }
            }
        }

        protected void SetAuthorDetails(IUserGeneratedContent vm)
        {
            UserProfile authorProfile = GetCachedProfileByUserId(vm.UserId);
            if (authorProfile != null)
            {
                vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId, 40);
                vm.AuthorName = authorProfile.Name;
            }
        }

        #endregion Generics

        #region Game

        public void SetGameCache(Guid id, Game value)
        {
            cacheService.Set<string, Game>(FormatObjectCacheId("game", id), value);
        }

        public void SetGameCache(Guid id, GameViewModel viewModel)
        {
            Game model = mapper.Map<Game>(viewModel);

            SetGameCache(id, model);
        }

        public GameViewModel GetGameWithCache(IDomainService<Game> domainService, Guid id)
        {
            Game model = GetObjectFromCache<Game>(id, "game");

            if (model == null)
            {
                model = domainService.GetById(id);
            }

            GameViewModel viewModel = mapper.Map<GameViewModel>(model);

            return viewModel;
        }

        #endregion Game

        private string FormatProfileCacheId(Guid userId)
        {
            return String.Format("profile_{0}", userId.ToString());
        }

        private string FormatObjectCacheId(string preffix, Guid id)
        {
            return String.Format("{0}_{1}", preffix, id.ToString());
        }
    }
}