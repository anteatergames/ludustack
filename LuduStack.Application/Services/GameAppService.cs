﻿using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.Specifications;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GameAppService : ProfileBaseAppService, IGameAppService
    {
        public GameAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountGameQuery, int>(new CountGameQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<GameViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<Game> allModels = await mediator.Query<GetGameQuery, IEnumerable<Game>>(new GetGameQuery());

                IEnumerable<GameViewModel> vms = mapper.Map<IEnumerable<Game>, IEnumerable<GameViewModel>>(allModels);

                return new OperationResultListVo<GameViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = await mediator.Query<GetGameIdsQuery, IEnumerable<Guid>>(new GetGameIdsQuery());

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameViewModel>> GetById(Guid currentUserId, Guid id)
        {
            return await GetById(currentUserId, id, false);
        }

        public async Task<OperationResultVo<GameViewModel>> GetById(Guid currentUserId, Guid id, bool forEdit)
        {
            try
            {
                Game model = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(id));

                GameViewModel vm = mapper.Map<GameViewModel>(model);

                vm.LikeCount = model.Likes.SafeCount(x => x.GameId == vm.Id);
                vm.FollowerCount = model.Followers.SafeCount(x => x.GameId == vm.Id);

                vm.CurrentUserLiked = model.Likes.SafeAny(x => x.GameId == vm.Id && x.UserId == currentUserId);
                vm.CurrentUserFollowing = model.Followers.SafeAny(x => x.GameId == vm.Id && x.UserId == currentUserId);

                UserProfileEssentialVo authorProfile = await GetCachedEssentialProfileByUserId(vm.UserId);
                if (authorProfile != null)
                {
                    vm.AuthorName = authorProfile.Name;
                    vm.UserHandler = authorProfile.Handler;
                }

                if (forEdit)
                {
                    FormatExternalLinksForEdit(ref vm);
                }

                await FormatExternalLinks(vm);

                FilCharacteristics(vm);

                if (string.IsNullOrWhiteSpace(vm.ThumbnailUrl) || Constants.DefaultGameThumbnail.Contains(vm.ThumbnailUrl) || vm.ThumbnailUrl.Contains(Constants.DefaultGameThumbnail))
                {
                    vm.ThumbnailUrl = Constants.DefaultGameThumbnail;
                }

                return new OperationResultVo<GameViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameViewModel>(ex.Message);
            }
        }

        public OperationResultVo<GameViewModel> CreateNew(Guid currentUserId)
        {
            GameViewModel vm = new GameViewModel
            {
                Engine = GameEngine.Unity,
                UserId = currentUserId,
                CoverImageUrl = Constants.DefaultGameCoverImage,
                ThumbnailUrl = Constants.DefaultGameThumbnail
            };

            FormatExternalLinksForEdit(ref vm);

            vm.Characteristics = Enum.GetValues(typeof(GameCharacteristcs)).Cast<GameCharacteristcs>().Where(x => x != GameCharacteristcs.NotInformed).Select(x => new GameCharacteristicVo { Characteristic = x, Available = false }).ToList();

            return new OperationResultVo<GameViewModel>(vm);
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                Game model;
                bool createTeam = viewModel.TeamId == Guid.Empty;

                ISpecification<GameViewModel> spec = new UserMustBeAuthenticatedSpecification<GameViewModel>(currentUserId)
                    .And(new UserIsOwnerSpecification<GameViewModel>(currentUserId));

                if (!spec.IsSatisfiedBy(viewModel))
                {
                    return new OperationResultVo<Guid>(spec.ErrorMessage);
                }

                Game existing = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Game>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveGameCommand(currentUserId, model, createTeam));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

                return new OperationResultVo<Guid>(model.Id, pointsEarned, "Game saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<IEnumerable<GameListItemViewModel>> GetLatest(Guid currentUserId, int count, Guid userId, Guid? teamId, GameGenre genre)
        {
            IEnumerable<Game> allModels = await mediator.Query<GetGameQuery, IEnumerable<Game>>(new GetGameQuery(count, genre, userId, teamId));

            List<GameListItemViewModel> vms = allModels.AsQueryable().ProjectTo<GameListItemViewModel>(mapper.ConfigurationProvider).ToList();

            IEnumerable<Guid> userIds = vms.Select(x => x.UserId);
            List<UserProfileEssentialVo> authorProfiles = await GetCachedEssentialProfilesByUserIds(userIds);

            foreach (GameListItemViewModel item in vms)
            {
                item.ThumbnailUrl = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.Full);
                item.ThumbnailResponsive = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.Responsive);
                item.ThumbnailLquip = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.LowQuality);
                item.DeveloperImageUrl = UrlFormatter.ProfileImage(item.UserId, Constants.TinyAvatarSize);

                UserProfileEssentialVo authorProfile = authorProfiles.FirstOrDefault(x => x.UserId == item.UserId);
                if (authorProfile != null)
                {
                    item.DeveloperName = authorProfile?.Name;
                    item.DeveloperHandler = authorProfile?.Handler;
                }
            }

            return vms;
        }

        private static string SetFeaturedImage(Guid userId, string thumbnailUrl, ImageRenderType imageType)
        {
            if (string.IsNullOrWhiteSpace(thumbnailUrl) || Constants.DefaultGameThumbnail.NoExtension().Contains(thumbnailUrl.NoExtension()))
            {
                return Constants.DefaultGameThumbnail;
            }
            else
            {
                switch (imageType)
                {
                    case ImageRenderType.LowQuality:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, 278, 10);

                    case ImageRenderType.Responsive:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, 278);
                }
            }
        }

        public async Task<IEnumerable<SelectListItemVo>> GetByUser(Guid userId)
        {
            IEnumerable<Game> allModels = await mediator.Query<GetGameByUserIdQuery, IEnumerable<Game>>(new GetGameByUserIdQuery(userId));

            List<SelectListItemVo> vms = mapper.Map<IEnumerable<Game>, IEnumerable<SelectListItemVo>>(allModels).ToList();

            return vms;
        }

        public async Task<OperationResultVo> GameFollow(Guid currentUserId, Guid gameId)
        {
            try
            {
                if (currentUserId == Guid.Empty)
                {
                    return new OperationResultVo("You must be logged in to follow a game");
                }

                CommandResult<int> result = await mediator.SendCommand<FollowGameCommand, int>(new FollowGameCommand(currentUserId, gameId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<int>(0, false, message);
                }

                return new OperationResultVo<int>(result.Result);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GameUnfollow(Guid currentUserId, Guid gameId)
        {
            try
            {
                if (currentUserId == Guid.Empty)
                {
                    return new OperationResultVo("You must be logged in to unfollow a game");
                }

                CommandResult<int> result = await mediator.SendCommand<UnfollowGameCommand, int>(new UnfollowGameCommand(currentUserId, gameId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<int>(0, false, message);
                }

                return new OperationResultVo<int>(result.Result);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GameLike(Guid currentUserId, Guid gameId)
        {
            try
            {
                CommandResult<int> result = await mediator.SendCommand<LikeGameCommand, int>(new LikeGameCommand(currentUserId, gameId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<int>(0, false, message);
                }

                return new OperationResultVo<int>(result.Result);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GameUnlike(Guid currentUserId, Guid gameId)
        {
            try
            {
                CommandResult<int> result = await mediator.SendCommand<UnlikeGameCommand, int>(new UnlikeGameCommand(currentUserId, gameId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<int>(0, false, message);
                }

                return new OperationResultVo<int>(result.Result);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private async Task FormatExternalLinks(GameViewModel vm)
        {
            IEnumerable<UserProfile> profiles = await mediator.Query<GetUserProfileByUserIdQuery, IEnumerable<UserProfile>>(new GetUserProfileByUserIdQuery(vm.UserId));
            UserProfile authorProfile = profiles.FirstOrDefault();
            ExternalLinkVo itchProfile = authorProfile.ExternalLinks.FirstOrDefault(x => x.Provider == ExternalLinkProvider.ItchIo);

            foreach (ExternalLinkBaseViewModel item in vm.ExternalLinks)
            {
                ExternalLinkInfoAttribute uiInfo = item.Provider.GetAttributeOfType<ExternalLinkInfoAttribute>();
                item.Display = uiInfo.Display;
                item.IconClass = uiInfo.Class;
                item.ColorClass = uiInfo.ColorClass;
                item.IsStore = uiInfo.IsStore;
                item.Order = uiInfo.Order;

                switch (item.Provider)
                {
                    case ExternalLinkProvider.Website:
                        item.Value = UrlFormatter.Website(item.Value);
                        break;

                    case ExternalLinkProvider.Facebook:
                        item.Value = UrlFormatter.Facebook(item.Value);
                        break;

                    case ExternalLinkProvider.Twitter:
                        item.Value = UrlFormatter.Twitter(item.Value);
                        break;

                    case ExternalLinkProvider.Instagram:
                        item.Value = UrlFormatter.Instagram(item.Value);
                        break;

                    case ExternalLinkProvider.Youtube:
                        item.Value = UrlFormatter.Youtube(item.Value);
                        break;

                    case ExternalLinkProvider.XboxLive:
                        item.Value = UrlFormatter.XboxLiveGame(item.Value);
                        break;

                    case ExternalLinkProvider.PlaystationStore:
                        item.Value = UrlFormatter.PlayStationStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.Steam:
                        item.Value = UrlFormatter.SteamGame(item.Value);
                        break;

                    case ExternalLinkProvider.GameJolt:
                        item.Value = UrlFormatter.GameJoltGame(item.Value);
                        break;

                    case ExternalLinkProvider.ItchIo:
                        item.Value = UrlFormatter.ItchIoGame(itchProfile?.Value, item.Value);
                        break;

                    case ExternalLinkProvider.GamedevNet:
                        item.Value = UrlFormatter.GamedevNetGame(item.Value);
                        break;

                    case ExternalLinkProvider.IndieDb:
                        item.Value = UrlFormatter.IndieDbGame(item.Value);
                        break;

                    case ExternalLinkProvider.UnityConnect:
                        item.Value = UrlFormatter.UnityConnectGame(item.Value);
                        break;

                    case ExternalLinkProvider.GooglePlayStore:
                        item.Value = UrlFormatter.GooglePlayStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.AppleAppStore:
                        item.Value = UrlFormatter.AppleAppStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.IndiExpo:
                        item.Value = UrlFormatter.IndiExpoGame(item.Value);
                        break;

                    case ExternalLinkProvider.Discord:
                        item.Value = UrlFormatter.DiscordGame(item.Value);
                        break;
                }
            }
        }

        private static void FormatExternalLinksForEdit(ref GameViewModel vm)
        {
            foreach (ExternalLinkProvider provider in Enum.GetValues(typeof(ExternalLinkProvider)))
            {
                ExternalLinkBaseViewModel existingProvider = vm.ExternalLinks.FirstOrDefault(x => x.Provider == provider);
                ExternalLinkInfoAttribute uiInfo = provider.GetAttributeOfType<ExternalLinkInfoAttribute>();

                if (uiInfo.Type != ExternalLinkType.ProfileOnly)
                {
                    if (existingProvider == null)
                    {
                        ExternalLinkBaseViewModel placeHolder = new ExternalLinkBaseViewModel
                        {
                            EntityId = vm.Id,
                            Type = uiInfo.Type,
                            Provider = provider,
                            Order = uiInfo.Order,
                            Display = uiInfo.Display,
                            IconClass = uiInfo.Class,
                            ColorClass = uiInfo.ColorClass,
                            IsStore = uiInfo.IsStore
                        };

                        vm.ExternalLinks.Add(placeHolder);
                    }
                    else
                    {
                        existingProvider.Display = uiInfo.Display;
                        existingProvider.IconClass = uiInfo.Class;
                        existingProvider.Order = uiInfo.Order;
                    }
                }
            }

            vm.ExternalLinks = vm.ExternalLinks.OrderByDescending(x => x.Type).ThenBy(x => x.Provider).ToList();
        }

        private static void FilCharacteristics(GameViewModel vm)
        {
            if (vm.Characteristics == null)
            {
                vm.Characteristics = new List<GameCharacteristicVo>();
            }
            List<GameCharacteristcs> allBenefits = Enum.GetValues(typeof(GameCharacteristcs)).Cast<GameCharacteristcs>().Where(x => x != GameCharacteristcs.NotInformed).ToList();

            foreach (GameCharacteristcs characteristic in allBenefits)
            {
                if (!vm.Characteristics.Any(x => x.Characteristic == characteristic))
                {
                    vm.Characteristics.Add(new GameCharacteristicVo { Characteristic = characteristic, Available = false });
                }
            }
        }
    }
}