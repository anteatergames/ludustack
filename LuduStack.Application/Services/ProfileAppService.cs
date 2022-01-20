using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.UserContent;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.Specifications;
using LuduStack.Domain.Specifications.Follow;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ProfileAppService : ProfileBaseAppService, IProfileAppService
    {
        public ProfileAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountUserProfileQuery, int>(new CountUserProfileQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ProfileViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<UserProfile> profiles = await mediator.Query<GetAllUserProfilesQuery, IEnumerable<UserProfile>>(new GetAllUserProfilesQuery());

                IEnumerable<ProfileViewModel> vms = mapper.Map<IEnumerable<UserProfile>, IEnumerable<ProfileViewModel>>(profiles);

                foreach (ProfileViewModel vm in vms)
                {
                    UserProfile model = profiles.First(x => x.UserId == vm.UserId);

                    vm.ProfileImageUrl = UrlFormatter.ProfileImage(vm.UserId, 84);
                    vm.CoverImageUrl = UrlFormatter.ProfileCoverImage(vm.UserId, vm.Id, vm.LastUpdateDate, model.HasCoverImage, 300);
                }

                return new OperationResultListVo<ProfileViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ProfileViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ProfileViewModel>> GetAllEssential(Guid currentUserId, bool noCache)
        {
            try
            {
                IEnumerable<Guid> allIds = profileDomainService.GetAllUserIds();

                List<UserProfileEssentialVo> profiles = await GetCachedEssentialProfilesByUserIds(allIds, noCache);
                IEnumerable<ProfileViewModel> vms = mapper.Map<IEnumerable<UserProfileEssentialVo>, IEnumerable<ProfileViewModel>>(profiles);

                foreach (ProfileViewModel vm in vms)
                {
                    UserProfileEssentialVo model = profiles.First(x => x.UserId == vm.UserId);

                    vm.ProfileImageUrl = UrlFormatter.ProfileImage(vm.UserId, Constants.HugeAvatarSize);
                    vm.CoverImageUrl = UrlFormatter.ProfileCoverImage(vm.UserId, vm.Id, vm.LastUpdateDate, model.HasCoverImage, Constants.ProfileCoverSize);
                }

                return new OperationResultListVo<ProfileViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ProfileViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = await mediator.Query<GetUserProfileIdsQuery, IEnumerable<Guid>>(new GetUserProfileIdsQuery());

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<string>> GetAllHandlers(Guid currentUserId)
        {
            try
            {
                IEnumerable<string> allHandlers = await mediator.Query<GetUserProfileHandlersQuery, IEnumerable<string>>(new GetUserProfileHandlersQuery());

                return new OperationResultListVo<string>(allHandlers);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<string>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ProfileViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                UserProfile model = await mediator.Query<GetUserProfileByIdQuery, UserProfile>(new GetUserProfileByIdQuery(id));

                ProfileViewModel vm = mapper.Map<ProfileViewModel>(model);

                return new OperationResultVo<ProfileViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ProfileViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteUserProfileCommand(currentUserId, id));

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ProfileViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                UserProfile model;

                if (viewModel.Bio.Contains(Constants.DefaultProfileDescription))
                {
                    viewModel.Bio = string.Format("{0} {1}", viewModel.Name, Constants.DefaultProfileDescription);
                }

                viewModel.ExternalLinks.RemoveAll(x => string.IsNullOrWhiteSpace(x.Value));

                UserProfile existing = await mediator.Query<GetUserProfileByIdQuery, UserProfile>(new GetUserProfileByIdQuery(viewModel.Id));
                if (existing == null)
                {
                    model = mapper.Map<UserProfile>(viewModel);
                }
                else
                {
                    model = mapper.Map(viewModel, existing);

                    if (!string.IsNullOrWhiteSpace(existing.Handler))
                    {
                        model.Handler = existing.Handler;
                    }
                }

                bool hasCoverImage = !viewModel.CoverImageUrl.Equals(Constants.DefaultProfileCoverImage);

                CommandResult result = await mediator.SendCommand(new SaveUserProfileCommand(currentUserId, model, hasCoverImage));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

                UserProfileEssentialVo profileVo = mapper.Map<UserProfileEssentialVo>(model);

                SetProfileCache(viewModel.UserId, profileVo);

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        #region IProfileAppService

        public async Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type)
        {
            return await Get(userId, userId, string.Empty, type, false);
        }

        public async Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type, bool forEdit)
        {
            return await Get(userId, userId, string.Empty, type, forEdit);
        }

        public async Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type)
        {
            return await Get(currentUserId, userId, userHandler, type, false);
        }

        public async Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type, bool forEdit)
        {
            ProfileViewModel vm = new ProfileViewModel();

            UserProfile model = await profileDomainService.Get(userId, userHandler, type);

            if (model != null)
            {
                vm = mapper.Map(model, vm);
            }
            else
            {
                return null;
            }

            SetImages(vm, model.HasCoverImage);

            vm.Counters.Games = await mediator.Query<CountGameQuery, int>(new CountGameQuery(x => x.UserId == vm.UserId));
            vm.Counters.Posts = await mediator.Query<CountUserContentQuery, int>(new CountUserContentQuery(x => x.UserId == vm.UserId));
            vm.Counters.Comments = await mediator.Query<CountCommentsQuery, int>(new CountCommentsQuery(x => x.UserId == vm.UserId));

            vm.Counters.Followers = model.Followers.SafeCount();
            vm.Counters.Following = profileDomainService.CountFollows(vm.UserId);
            int connectionsToUser = profileDomainService.CountConnections(x => x.TargetUserId == vm.UserId && x.ApprovalDate.HasValue);
            int connectionsFromUser = profileDomainService.CountConnections(x => x.UserId == vm.UserId && x.ApprovalDate.HasValue);

            vm.Counters.Connections = connectionsToUser + connectionsFromUser;

            if (vm.UserId != currentUserId)
            {
                vm.CurrentUserFollowing = model.Followers.SafeAny(x => x.UserId == currentUserId);

                UserConnectionVo connectionDetails = profileDomainService.GetConnectionDetails(currentUserId, vm.UserId);

                vm.ConnectionControl.CurrentUserConnected = connectionDetails != null && connectionDetails.Accepted;
                vm.ConnectionControl.CurrentUserWantsToConnect = connectionDetails != null && connectionDetails.Direction == UserConnectionDirection.ToUser && !connectionDetails.Accepted;
                vm.ConnectionControl.ConnectionIsPending = connectionDetails != null && !connectionDetails.Accepted;
                vm.ConnectionControl.ConnectionType = connectionDetails != null ? connectionDetails.ConnectionType : new UserConnectionType?();
            }

            if (forEdit)
            {
                FormatExternalLinksForEdit(ref vm);
            }

            FormatExternalLinks(vm);

            return vm;
        }

        #endregion IProfileAppService

        public ProfileViewModel GenerateNewOne(ProfileType type)
        {
            ProfileViewModel profile = new ProfileViewModel();

            RandomNumberGenerator randomGenerator = RandomNumberGenerator.Create();
            byte[] data = new byte[4];
            randomGenerator.GetBytes(data);
            int randomNumber = BitConverter.ToInt32(data);

            profile.Type = ProfileType.Personal;

            profile.Name = string.Format("NPC {0}", Math.Abs(randomNumber));
            profile.Motto = "It is dangerous out there, take this...";

            profile.Bio = string.Format("{0} {1}", profile.Name, Constants.DefaultProfileDescription);

            profile.ProfileImageUrl = Constants.DefaultAvatar;
            profile.CoverImageUrl = Constants.DefaultGameCoverImage;

            return profile;
        }

        public OperationResultVo Search(string term)
        {
            try
            {
                IQueryable<UserProfile> results = profileDomainService.Search(x => x.Name.ToLower().Contains(term.ToLower()));

                IQueryable<ProfileSearchViewModel> vms = results.ProjectTo<ProfileSearchViewModel>(mapper.ConfigurationProvider);

                return new OperationResultListVo<ProfileSearchViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultListVo<ProfileSearchViewModel> SearchUserCard(string term)
        {
            try
            {
                IQueryable<UserProfile> results = profileDomainService.Search(x => x.Name.ToLower().Contains(term.ToLower()));

                List<ProfileSearchViewModel> vms = results.ProjectTo<ProfileSearchViewModel>(mapper.ConfigurationProvider).ToList();

                foreach (ProfileSearchViewModel item in vms)
                {
                    item.ProfileImageUrl = UrlFormatter.ProfileImage(item.UserId, Constants.HugeAvatarSize);
                    item.CoverImageUrl = UrlFormatter.ProfileCoverImage(item.UserId, item.Id, null, item.HasCoverImage, Constants.ProfileCoverSize);
                }

                return new OperationResultListVo<ProfileSearchViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ProfileSearchViewModel>(ex.Message);
            }
        }

        public OperationResultVo UserFollow(Guid currentUserId, Guid userId)
        {
            try
            {
                UserFollow model = new UserFollow
                {
                    UserId = currentUserId,
                    FollowUserId = userId
                };

                ISpecification<UserFollow> spec = new IdNotEmptySpecification<UserFollow>()
                    .And(new UserNotTheSameSpecification(userId));

                if (!spec.IsSatisfiedBy(model))
                {
                    return new OperationResultVo(false, spec.ErrorMessage);
                }

                bool alreadyFollowing = profileDomainService.CheckFollowing(userId, currentUserId);

                if (alreadyFollowing)
                {
                    return new OperationResultVo(false, "User already followed");
                }
                else
                {
                    profileDomainService.AddFollow(model);

                    unitOfWork.Commit();

                    int newCount = profileDomainService.CountFollows(userId);

                    return new OperationResultVo<int>(newCount);
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo UserUnfollow(Guid currentUserId, Guid userId)
        {
            try
            {
                if (currentUserId == Guid.Empty)
                {
                    return new OperationResultVo("You must be logged in to unfollow a user.");
                }
                else
                {
                    UserFollow existingFollow = profileDomainService.GetFollows(userId, currentUserId).FirstOrDefault();

                    if (existingFollow == null)
                    {
                        return new OperationResultVo(false, "You are not following this user.");
                    }
                    else
                    {
                        profileDomainService.RemoveFollow(existingFollow, userId);

                        unitOfWork.Commit();

                        int newCount = profileDomainService.CountFollows(userId);

                        return new OperationResultVo<int>(newCount);
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #region UserConnection

        public OperationResultVo GetConnectionsByUserId(Guid userId)
        {
            try
            {
                List<UserConnection> connectionsFromDb = profileDomainService.GetConnectionsByUserId(userId, true);

                List<UserConnectionViewModel> connections = mapper.Map<List<UserConnectionViewModel>>(connectionsFromDb);

                List<UserConnectionViewModel> connectionsFormatted = FormatConnections(userId, connections);

                return new OperationResultListVo<UserConnectionViewModel>(connectionsFormatted);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo Connect(Guid currentUserId, Guid userId, UserConnectionType connectionType)
        {
            try
            {
                UserConnection model = new UserConnection
                {
                    UserId = currentUserId,
                    TargetUserId = userId,
                    ConnectionType = connectionType
                };

                UserConnection existing = profileDomainService.GetConnection(currentUserId, userId);

                if (existing != null)
                {
                    return new OperationResultVo("You are already connected to this user!");
                }
                else
                {
                    profileDomainService.AddConnection(model);
                }

                unitOfWork.Commit();

                int newCount = profileDomainService.CountConnections(x => (x.TargetUserId == userId || x.UserId == userId) && x.ApprovalDate.HasValue);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo Disconnect(Guid currentUserId, Guid userId)
        {
            try
            {
                UserConnection toMe = profileDomainService.GetConnection(currentUserId, userId);
                UserConnection fromMe = profileDomainService.GetConnection(userId, currentUserId);

                if (toMe == null && fromMe == null)
                {
                    return new OperationResultVo("You are not connected to this user!");
                }
                else
                {
                    if (toMe != null)
                    {
                        profileDomainService.RemoveConnection(toMe.Id);
                    }
                    if (fromMe != null)
                    {
                        profileDomainService.RemoveConnection(fromMe.Id);
                    }
                }

                unitOfWork.Commit();

                int newCount = profileDomainService.CountConnections(x => x.TargetUserId == userId);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo Allow(Guid currentUserId, Guid userId)
        {
            try
            {
                UserConnection existing = profileDomainService.GetConnection(userId, currentUserId);

                if (existing == null)
                {
                    return new OperationResultVo("There is no connection requested by this user.");
                }
                else
                {
                    existing.ApprovalDate = DateTime.Now;

                    profileDomainService.UpdateConnection(existing);
                }

                unitOfWork.Commit();

                int newCount = profileDomainService.CountConnections(x => (x.TargetUserId == userId || x.UserId == userId) && x.ApprovalDate.HasValue);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo Deny(Guid currentUserId, Guid userId)
        {
            try
            {
                UserConnection existing = profileDomainService.GetConnection(userId, currentUserId);

                if (existing == null)
                {
                    return new OperationResultVo("There is no connection requested by this user.");
                }
                else
                {
                    profileDomainService.RemoveConnection(existing.Id);
                }

                unitOfWork.Commit();

                int newCount = profileDomainService.CountConnections(x => (x.TargetUserId == userId || x.UserId == userId) && x.ApprovalDate.HasValue);

                return new OperationResultVo<int>(newCount);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion UserConnection

        private void SetImages(ProfileViewModel vm, bool hasCoverImage)
        {
            vm.ProfileImageUrl = UrlFormatter.ProfileImage(vm.UserId, vm.LastUpdateDate, 84);
            vm.CoverImageUrl = UrlFormatter.ProfileCoverImage(vm.UserId, vm.Id, vm.LastUpdateDate, hasCoverImage, 1110);
        }

        private static void FormatConnectionImages(UserConnectionViewModel item, UserProfileEssentialVo profile)
        {
            item.ProfileImageUrl = UrlFormatter.ProfileImage(item.TargetUserId);
            item.CoverImageUrl = UrlFormatter.ProfileCoverImage(item.TargetUserId, profile.Id, profile.LastUpdateDate, profile.HasCoverImage);
        }

        private List<UserConnectionViewModel> FormatConnections(Guid userId, IEnumerable<UserConnectionViewModel> connections)
        {
            List<UserConnectionViewModel> newList = new List<UserConnectionViewModel>();

            IEnumerable<UserConnectionViewModel> connectionsFromMe = connections.Where(x => x.UserId == userId && x.ApprovalDate.HasValue).ToList();
            IEnumerable<UserConnectionViewModel> connectionsToMe = connections.Where(x => x.TargetUserId == userId && x.ApprovalDate.HasValue).ToList();

            foreach (UserConnectionViewModel item in connectionsFromMe)
            {
                if (!newList.Any(x => x.UserId == item.TargetUserId))
                {
                    UserProfileEssentialVo profile = profileDomainService.GetBasicDataByUserId(item.TargetUserId);

                    item.UserId = userId;
                    item.UserHandler = profile.Handler;
                    item.TargetUserName = profile.Name;
                    item.Location = profile.Location;
                    item.CreateDate = profile.CreateDate;

                    FormatConnectionImages(item, profile);

                    newList.Add(item);
                }
            }

            foreach (UserConnectionViewModel item in connectionsToMe)
            {
                if (!newList.Any(x => x.UserId == item.UserId))
                {
                    UserProfileEssentialVo profile = profileDomainService.GetBasicDataByUserId(item.UserId);

                    item.TargetUserId = item.UserId;
                    item.UserId = userId;
                    item.UserHandler = profile.Handler;
                    item.TargetUserName = profile.Name;
                    item.ProfileId = profile.Id;
                    item.Location = profile.Location;
                    item.CreateDate = profile.CreateDate;

                    FormatConnectionImages(item, profile);

                    newList.Add(item);
                }
            }

            return newList;
        }

        private void FormatExternalLinks(ProfileViewModel vm)
        {
            foreach (ExternalLinkBaseViewModel item in vm.ExternalLinks)
            {
                ExternalLinkInfoAttribute uiInfo = item.Provider.GetAttributeOfType<ExternalLinkInfoAttribute>();
                item.Display = uiInfo.Display;
                item.IconClass = uiInfo.Class;
                item.ColorClass = uiInfo.ColorClass;
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
                        item.Value = UrlFormatter.XboxLiveProfile(item.Value);
                        break;

                    case ExternalLinkProvider.PlaystationStore:
                        item.Value = UrlFormatter.PlayStationStoreProfile(item.Value);
                        break;

                    case ExternalLinkProvider.Steam:
                        item.Value = UrlFormatter.SteamGame(item.Value);
                        break;

                    case ExternalLinkProvider.GameJolt:
                        item.Value = UrlFormatter.GameJoltProfile(item.Value);
                        break;

                    case ExternalLinkProvider.ItchIo:
                        item.Value = UrlFormatter.ItchIoProfile(item.Value);
                        break;

                    case ExternalLinkProvider.GamedevNet:
                        item.Value = UrlFormatter.GamedevNetProfile(item.Value);
                        break;

                    case ExternalLinkProvider.IndieDb:
                        item.Value = UrlFormatter.IndieDbPofile(item.Value);
                        break;

                    case ExternalLinkProvider.UnityConnect:
                        item.Value = UrlFormatter.UnityConnectProfile(item.Value);
                        break;

                    case ExternalLinkProvider.GooglePlayStore:
                        item.Value = UrlFormatter.GooglePlayStoreProfile(item.Value);
                        break;

                    case ExternalLinkProvider.AppleAppStore:
                        item.Value = UrlFormatter.AppleAppStoreProfile(item.Value);
                        break;

                    case ExternalLinkProvider.IndiExpo:
                        item.Value = UrlFormatter.IndiExpoProfile(item.Value);
                        break;

                    case ExternalLinkProvider.Artstation:
                        item.Value = UrlFormatter.ArtstationProfile(item.Value);
                        break;

                    case ExternalLinkProvider.DeviantArt:
                        item.Value = UrlFormatter.DeviantArtProfile(item.Value);
                        break;

                    case ExternalLinkProvider.DevTo:
                        item.Value = UrlFormatter.DevToProfile(item.Value);
                        break;

                    case ExternalLinkProvider.GitHub:
                        item.Value = UrlFormatter.GitHubProfile(item.Value);
                        break;

                    case ExternalLinkProvider.HackerRank:
                        item.Value = UrlFormatter.HackerRankProfile(item.Value);
                        break;

                    case ExternalLinkProvider.LinkedIn:
                        item.Value = UrlFormatter.LinkedInProfile(item.Value);
                        break;

                    case ExternalLinkProvider.Patreon:
                        item.Value = UrlFormatter.PatreonProfile(item.Value);
                        break;

                    case ExternalLinkProvider.Medium:
                        item.Value = UrlFormatter.MediumProfile(item.Value);
                        break;

                    case ExternalLinkProvider.Discord:
                        item.Value = UrlFormatter.DiscordProfile(item.Value);
                        break;

                    default:
                        item.Value = UrlFormatter.Website(item.Value);
                        break;
                }
            }
        }

        private static void FormatExternalLinksForEdit(ref ProfileViewModel vm)
        {
            foreach (ExternalLinkProvider provider in Enum.GetValues(typeof(ExternalLinkProvider)))
            {
                ExternalLinkBaseViewModel existingProvider = vm.ExternalLinks.FirstOrDefault(x => x.Provider == provider);
                ExternalLinkInfoAttribute uiInfo = provider.GetAttributeOfType<ExternalLinkInfoAttribute>();

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

            vm.ExternalLinks = vm.ExternalLinks.OrderBy(x => x.Order).ToList();
        }
    }
}