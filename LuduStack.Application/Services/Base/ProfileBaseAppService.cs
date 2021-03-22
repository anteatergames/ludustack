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
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public abstract class ProfileBaseAppService : BaseAppService, IProfileBaseAppService
    {
        protected readonly IProfileDomainService profileDomainService;

        protected ProfileBaseAppService(IMediatorHandler mediator, IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon.Mapper, profileBaseAppServiceCommon.UnitOfWork, profileBaseAppServiceCommon.Mediator, profileBaseAppServiceCommon.CacheService)
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

        public async Task<GameViewModel> GetGameWithCache(IDomainService<Game> domainService, Guid id)
        {
            Game model = GetObjectFromCache<Game>(id, "game");

            if (model == null)
            {
                model = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(id));
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