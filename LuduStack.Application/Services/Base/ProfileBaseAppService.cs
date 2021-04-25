﻿using CountryData;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public abstract class ProfileBaseAppService : BaseAppService, IProfileBaseAppService
    {
        protected readonly IProfileDomainService profileDomainService;

        protected ProfileBaseAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon.Mapper, profileBaseAppServiceCommon.UnitOfWork, profileBaseAppServiceCommon.Mediator, profileBaseAppServiceCommon.CacheService)
        {
            profileDomainService = profileBaseAppServiceCommon.ProfileDomainService;
        }

        #region Profile

        public void SetProfileCache(Guid userId, UserProfile value)
        {
            cacheService.Set<string, UserProfile>(FormatProfileCacheId(userId), value);
        }

        public void SetProfileCache(Guid userId, UserProfileEssentialVo value)
        {
            cacheService.Set<string, UserProfileEssentialVo>(FormatProfileCacheId(userId), value);
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

        private UserProfileEssentialVo GetEssentialProfileFromCache(Guid userId)
        {
            UserProfileEssentialVo fromCache = cacheService.Get<string, UserProfileEssentialVo>(FormatProfileCacheId(userId));

            return fromCache;
        }

        protected async Task<UserProfile> GetCachedProfileByUserId(Guid userId)
        {
            return await GetCachedProfileByUserId(userId, false);
        }

        protected async Task<UserProfile> GetCachedProfileByUserId(Guid userId, bool noCache)
        {
            UserProfile profile = noCache ? null : GetProfileFromCache(userId);

            if (profile == null)
            {
                IEnumerable<UserProfile> allUserProfiles = await mediator.Query<GetUserProfileByUserIdQuery, IEnumerable<UserProfile>>(new GetUserProfileByUserIdQuery(userId));

                UserProfile profileFromDb = allUserProfiles.FirstOrDefault();

                if (profileFromDb != null)
                {
                    SetProfileCache(userId, profileFromDb);
                    profile = profileFromDb;
                }
            }

            return profile;
        }

        protected async Task<List<UserProfileEssentialVo>> GetCachedProfilesByUserIds(Guid userIds)
        {
            return await GetCachedProfilesByUserIds(new List<Guid> { userIds }, false);
        }

        protected async Task<List<UserProfileEssentialVo>> GetCachedProfilesByUserIds(Guid userIds, bool noCache)
        {
            return await GetCachedProfilesByUserIds(new List<Guid> { userIds }, noCache);
        }

        protected async Task<List<UserProfileEssentialVo>> GetCachedProfilesByUserIds(IEnumerable<Guid> userIds)
        {
            return await GetCachedProfilesByUserIds(userIds, false);
        }

        protected async Task<List<UserProfileEssentialVo>> GetCachedProfilesByUserIds(IEnumerable<Guid> userIds, bool noCache)
        {
            List<UserProfileEssentialVo> profiles = new List<UserProfileEssentialVo>();

            var userIdsToCache = new List<Guid>();
            foreach (var userId in userIds)
            {
                var profile = noCache ? null : GetEssentialProfileFromCache(userId);

                if (profile == null)
                {
                    userIdsToCache.Add(userId);
                }
                else
                {
                    profiles.Add(profile);
                }
            }

            IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIdsToCache));

            foreach (var profile in userProfiles)
            {
                var profileFromDb = userProfiles.FirstOrDefault(x => x.UserId == profile.UserId);

                if (profileFromDb != null)
                {
                    SetProfileCache(profile.UserId, profileFromDb);

                    profiles.Add(profileFromDb);
                }
            }

            return profiles;
        }

        public async Task<ProfileViewModel> GetUserProfileWithCache(Guid userId)
        {
            UserProfile model = GetProfileFromCache(userId);

            if (model == null)
            {
                IEnumerable<UserProfile> allUserProfiles = await mediator.Query<GetUserProfileByUserIdQuery, IEnumerable<UserProfile>>(new GetUserProfileByUserIdQuery(userId));

                model = allUserProfiles.FirstOrDefault();
            }

            ProfileViewModel viewModel = mapper.Map<ProfileViewModel>(model);

            return viewModel;
        }

        #endregion Profile

        #region Generics

        private T GetObjectFromCache<T>(Guid id, string preffix) where T : Entity
        {
            T fromCache = cacheService.Get<string, T>(FormatObjectCacheId(preffix, id));

            return fromCache;
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
                    case ImageRenderType.Small:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, 350, 0);

                    case ImageRenderType.LowQuality:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, 278, 10);

                    case ImageRenderType.Responsive:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return UrlFormatter.Image(userId, ImageType.CourseThumbnail, thumbnailUrl, false, 0, 0);
                }
            }
        }

        protected async Task SetAuthorDetails(IUserGeneratedContent vm)
        {
            UserProfile authorProfile = await GetCachedProfileByUserId(vm.UserId);
            if (authorProfile != null)
            {
                vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId, 40);
                vm.AuthorName = authorProfile.Name;
                vm.UserHandler = authorProfile.Handler;
            }
        }

        protected void SetAuthorDetails(Guid currentUserId, IUserGeneratedContent vm, UserProfileEssentialVo userProfile)
        {
            SetAuthorDetails(currentUserId, vm, new List<UserProfileEssentialVo> { userProfile });
        }

        protected void SetAuthorDetails(Guid currentUserId, IUserGeneratedContent vm, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            if (vm.Id == Guid.Empty || vm.UserId == Guid.Empty)
            {
                vm.UserId = currentUserId;
            }

            var authorProfile = userProfiles.FirstOrDefault(x => x.UserId == vm.UserId);
            if (authorProfile != null)
            {
                vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId, 40);
                vm.AuthorName = authorProfile.Name;
                vm.UserHandler = authorProfile.Handler;
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

        public async Task<GameViewModel> GetGameWithCache(Guid gameId)
        {
            Game model = GetObjectFromCache<Game>(gameId, "game");

            if (model == null)
            {
                model = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(gameId));
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