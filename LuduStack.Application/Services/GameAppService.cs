﻿using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.Game;
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
        private readonly IGamificationDomainService gamificationDomainService;
        private readonly ITeamDomainService teamDomainService;

        private readonly IGameDomainService gameDomainService;

        public GameAppService(IMediatorHandler mediator
            , IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , ITeamDomainService teamDomainService
            , IGameDomainService gameDomainService
            , IGamificationDomainService gamificationDomainService) : base(mediator, profileBaseAppServiceCommon)
        {
            this.gameDomainService = gameDomainService;
            this.teamDomainService = teamDomainService;
            this.gamificationDomainService = gamificationDomainService;
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

        public Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = gameDomainService.GetAllIds();

                return Task.FromResult(new OperationResultListVo<Guid>(allIds));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OperationResultListVo<Guid>(ex.Message));
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

                UserProfile authorProfile = GetCachedProfileByUserId(vm.UserId);
                vm.AuthorName = authorProfile.Name;

                if (forEdit)
                {
                    FormatExternalLinksForEdit(ref vm);
                }

                FormatExternalLinks(vm);

                FilCharacteristics(vm);

                if (Constants.DefaultGameThumbnail.Contains(vm.ThumbnailUrl) || vm.ThumbnailUrl.Contains(Constants.DefaultGameThumbnail))
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
                Game game;
                Team newTeam = null;
                bool createTeam = viewModel.TeamId == Guid.Empty;

                ISpecification<GameViewModel> spec = new UserMustBeAuthenticatedSpecification<GameViewModel>(currentUserId)
                    .And(new UserIsOwnerSpecification<GameViewModel>(currentUserId));

                if (!spec.IsSatisfiedBy(viewModel))
                {
                    return new OperationResultVo<Guid>(spec.ErrorMessage);
                }

                if (createTeam)
                {
                    newTeam = teamDomainService.GenerateNewTeam(currentUserId);
                    newTeam.Name = String.Format("Team {0}", viewModel.Title);
                    teamDomainService.Add(newTeam);
                }

                viewModel.ExternalLinks.RemoveAll(x => String.IsNullOrWhiteSpace(x.Value));

                Game existing = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    game = mapper.Map(viewModel, existing);
                }
                else
                {
                    game = mapper.Map<Game>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    gameDomainService.Add(game);

                    pointsEarned += gamificationDomainService.ProcessAction(viewModel.UserId, PlatformAction.GameAdd);
                }
                else
                {
                    gameDomainService.Update(game);
                }

                await unitOfWork.Commit();
                viewModel.Id = game.Id;

                if (createTeam && newTeam != null)
                {
                    game.TeamId = newTeam.Id;
                    gameDomainService.Update(game);
                    await unitOfWork.Commit();
                }

                return new OperationResultVo<Guid>(game.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                gameDomainService.Remove(id);
                await unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public IEnumerable<GameListItemViewModel> GetLatest(Guid currentUserId, int count, Guid userId, Guid? teamId, GameGenre genre)
        {
            IQueryable<Game> allModels = gameDomainService.Get(genre, userId, teamId);

            IOrderedQueryable<Game> ordered = allModels.OrderByDescending(x => x.CreateDate);

            IQueryable<Game> taken = ordered.Take(count);

            List<GameListItemViewModel> vms = taken.ProjectTo<GameListItemViewModel>(mapper.ConfigurationProvider).ToList();

            foreach (GameListItemViewModel item in vms)
            {
                item.ThumbnailUrl = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.Full);
                item.ThumbnailResponsive = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.Responsive);
                item.ThumbnailLquip = SetFeaturedImage(item.UserId, item.ThumbnailUrl, ImageRenderType.LowQuality);
                item.DeveloperImageUrl = UrlFormatter.ProfileImage(item.UserId, 40);

                UserProfile authorProfile = GetCachedProfileByUserId(item.UserId);
                item.DeveloperName = authorProfile?.Name;
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

        public OperationResultVo GameFollow(Guid currentUserId, Guid gameId)
        {
            try
            {
                if (currentUserId == Guid.Empty)
                {
                    return new OperationResultVo("You must be logged in to follow a game");
                }

                gameDomainService.Follow(currentUserId, gameId);

                unitOfWork.Commit();

                int newCount = gameDomainService.CountFollowers(gameId);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GameUnfollow(Guid currentUserId, Guid gameId)
        {
            try
            {
                if (currentUserId == Guid.Empty)
                {
                    return new OperationResultVo("You must be logged in to unfollow a game");
                }

                gameDomainService.Unfollow(currentUserId, gameId);

                unitOfWork.Commit();

                int newCount = gameDomainService.CountFollowers(gameId);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GameLike(Guid currentUserId, Guid gameId)
        {
            try
            {
                gameDomainService.Like(currentUserId, gameId);

                unitOfWork.Commit();

                int newCount = gameDomainService.CountLikes(gameId);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GameUnlike(Guid currentUserId, Guid gameId)
        {
            try
            {
                gameDomainService.Unlike(currentUserId, gameId);

                unitOfWork.Commit();

                int newCount = gameDomainService.CountLikes(gameId);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void FormatExternalLinks(GameViewModel vm)
        {
            ProfileViewModel authorProfile = GetUserProfileWithCache(vm.UserId);
            ExternalLinkBaseViewModel itchProfile = authorProfile.ExternalLinks.FirstOrDefault(x => x.Provider == ExternalLinkProvider.ItchIo);

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