using Ganss.XSS;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.GameJam;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GameJamAppService : ProfileBaseAppService, IGameJamAppService
    {
        public GameJamAppService(IProfileBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountGameJamQuery, int>(new CountGameJamQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<IEnumerable<SelectListItemVo>> GetSelectList(Guid userId)
        {
            IEnumerable<GameJam> allGroups = await mediator.Query<GetGameJamQuery, IEnumerable<GameJam>>(new GetGameJamQuery());

            IEnumerable<SelectListItemVo> list = allGroups.OrderBy(x => x.Name).Select(x => new SelectListItemVo
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return list;
        }

        public async Task<OperationResultListVo<GameJamViewModel>> GetAll(Guid currentUserId, bool currentUserIsAdmin)
        {
            try
            {
                IEnumerable<GameJamListItem> allModels = await mediator.Query<GetGameJamListQuery, IEnumerable<GameJamListItem>>(new GetGameJamListQuery());

                IEnumerable<GameJamViewModel> vms = mapper.Map<IEnumerable<GameJamListItem>, IEnumerable<GameJamViewModel>>(allModels);

                SetViewModelStates(currentUserId, currentUserIsAdmin, vms);

                vms = vms.OrderBy(x => x.CurrentPhase).ThenBy(x => x.SecondsToCountDown);

                return new OperationResultListVo<GameJamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameJamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<GameJamViewModel>> GetByUserId(Guid userId, bool currentUserIsAdmin)
        {
            try
            {
                IEnumerable<GameJam> allModels = await mediator.Query<GetGameJamQuery, IEnumerable<GameJam>>(new GetGameJamQuery(x => x.UserId == userId));

                IEnumerable<GameJamViewModel> vms = mapper.Map<IEnumerable<GameJam>, IEnumerable<GameJamViewModel>>(allModels);

                SetViewModelStates(userId, currentUserIsAdmin, vms);

                IOrderedEnumerable<GameJamViewModel> finalLisit = vms.OrderBy(x => x.CurrentPhase).ThenBy(x => x.StartDate).ThenBy(x => x.EntryDeadline).ThenBy(x => x.VotingEndDate).ThenBy(x => x.ResultDate);

                return new OperationResultListVo<GameJamViewModel>(finalLisit);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameJamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameJamViewModel>> GetForDetails(Guid currentUserId, bool currentUserIsAdmin, Guid id, string handler)
        {
            try
            {
                GameJam model = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(id, handler));

                if (model == null)
                {
                    return new OperationResultVo<GameJamViewModel>("Entity not found!");
                }

                GameJamViewModel vm = mapper.Map<GameJamViewModel>(model);

                UserProfileEssentialVo authorProfile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(model.UserId));
                IEnumerable<Guid> entries = await mediator.Query<GetEntriesUserIdsQuery, IEnumerable<Guid>>(new GetEntriesUserIdsQuery(x => x.GameJamId == model.Id));

                vm.AuthorName = authorProfile.Name;
                vm.AuthorHandler = authorProfile.Handler;

                SetGameJamState(DateTime.Now.ToLocalTime(), vm);

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();

                SanitizeHtml(vm, sanitizer);

                await SetJudges(vm);
                SetHighlights(vm);

                SetGameJamImagesToShow(vm, false);

                SetViewModelState(currentUserId, vm, entries);

                SetJamPermissions(currentUserId, currentUserIsAdmin, vm);

                return new OperationResultVo<GameJamViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameJamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameJamViewModel>> GetForEdit(Guid currentUserId, bool currentUserIsAdmin, Guid id)
        {
            try
            {
                GameJam model = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<GameJamViewModel>("Entity not found!");
                }

                bool userCanEdit = model.UserId == currentUserId || currentUserIsAdmin;

                if (!userCanEdit)
                {
                    return new OperationResultVo<GameJamViewModel>("You cannot edit this!");
                }

                GameJamViewModel vm = mapper.Map<GameJamViewModel>(model);

                await SetJudges(vm);

                SetCriteria(vm, false);

                SetGameJamImagesToShow(vm, true);

                SetJamPermissions(currentUserId, currentUserIsAdmin, vm);

                return new OperationResultVo<GameJamViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameJamViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteGameJamCommand(currentUserId, id));

                return new OperationResultVo(true, "That Game Jam is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameJamViewModel viewModel)
        {
            try
            {
                GameJam model;

                GameJam existing = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                    model.UserId = existing.UserId;
                }
                else
                {
                    if (viewModel.UserId == Guid.Empty)
                    {
                        viewModel.UserId = currentUserId;
                    }
                    model = mapper.Map<GameJam>(viewModel);
                }

                if (model.FeaturedImage == Constants.DefaultGamejamThumbnail)
                {
                    model.FeaturedImage = null;
                }

                if (model.BannerImage == Constants.DefaultGamejamThumbnail)
                {
                    model.BannerImage = null;
                }

                if (model.BackgroundImage == Constants.DefaultGamejamThumbnail)
                {
                    model.BackgroundImage = null;
                }

                CommandResult result = await mediator.SendCommand(new SaveGameJamCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Game Jam saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> CalculateResults(Guid currentUserId, Guid jamId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new CalculateResultsGameJamCommand(jamId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(jamId, false, message);
                }

                return new OperationResultVo<Guid>(jamId, result.PointsEarned, "Game Jam finished! Results available.");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameJamViewModel>> GenerateNew(Guid currentUserId)
        {
            try
            {
                GameJamViewModel newVm = new GameJamViewModel
                {
                    UserId = currentUserId,
                    StartDate = DateTime.Now.AddDays(7)
                };
                newVm.EntryDeadline = newVm.StartDate.AddDays(7);
                newVm.VotingEndDate = newVm.EntryDeadline.AddDays(7);
                newVm.ResultDate = newVm.VotingEndDate.AddDays(7);

                newVm.FeaturedImage = Constants.DefaultGamejamThumbnail;
                newVm.BannerImage = Constants.DefaultGamejamThumbnail;
                newVm.BackgroundImage = Constants.DefaultGamejamThumbnail;

                newVm.JudgesProfiles = new List<ProfileViewModel>();

                UserProfileEssentialVo myProfile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(currentUserId));
                ProfileViewModel profile = new ProfileViewModel
                {
                    UserId = myProfile.UserId,
                    Handler = myProfile.Handler,
                    Location = myProfile.Location,
                    CreateDate = myProfile.CreateDate,
                    Name = myProfile.Name,
                    ProfileImageUrl = UrlFormatter.ProfileImage(myProfile.UserId, Constants.HugeAvatarSize),
                    CoverImageUrl = UrlFormatter.ProfileCoverImage(myProfile.UserId, myProfile.Id, myProfile.LastUpdateDate, myProfile.HasCoverImage, Constants.ProfileCoverSize)
                };

                newVm.JudgesProfiles.Add(profile);

                SetCriteria(newVm, true);

                return new OperationResultVo<GameJamViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameJamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> ValidateHandler(Guid currentUserId, string handler, Guid id)
        {
            try
            {
                bool valid = await mediator.Query<CheckGameJamHandlerQuery, bool>(new CheckGameJamHandlerQuery(handler, id));

                return new OperationResultVo(valid);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultListVo<GameJamEntryViewModel>> GetEntriesByJam(Guid currentUserId, bool currentUserIsAdmin, string jamHandler, Guid jamId, bool submittedOnly)
        {
            try
            {
                GameJam gameJam = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(jamId));
                GameJamViewModel gameJamVm = mapper.Map<GameJamViewModel>(gameJam);

                SetGameJamState(DateTime.Now.ToLocalTime(), gameJamVm);

                IEnumerable<GameJamEntry> allModels = await mediator.Query<GetGameJamEntryListQuery, IEnumerable<GameJamEntry>>(new GetGameJamEntryListQuery(jamId, submittedOnly));

                IEnumerable<GameJamEntryViewModel> vms = mapper.Map<IEnumerable<GameJamEntry>, IEnumerable<GameJamEntryViewModel>>(allModels);

                List<GameJamEntryViewModel> vmList = vms.ToList();

                IEnumerable<Guid> gamesIds = vms.Where(x => x.GameId != Guid.Empty).Select(x => x.GameId);

                IEnumerable<Game> games = await mediator.Query<GetGamesByIdsQuery, IEnumerable<Game>>(new GetGamesByIdsQuery(gamesIds));

                IEnumerable<Guid> userIds = vmList.Select(x => x.UserId);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIds));

                foreach (GameJamEntryViewModel vm in vmList)
                {
                    if (vm.GameId != Guid.Empty)
                    {
                        Game relatedGame = games.FirstOrDefault(x => x.Id == vm.GameId);
                        if (relatedGame != null)
                        {
                            vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(relatedGame.UserId, relatedGame.ThumbnailUrl, ImageRenderType.Full);
                        }
                    }
                    else
                    {
                        vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, Constants.DefaultGameThumbnail, ImageRenderType.Full);
                    }

                    vm.JamHandler = jamHandler;

                    UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == vm.UserId);
                    if (authorProfile != null)
                    {
                        vm.UserHandler = authorProfile.Handler;
                        vm.AuthorName = authorProfile.Name;
                        vm.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, Constants.SmallAvatarSize);
                    }

                    SetVotes(currentUserId, gameJamVm, vm);

                    if (vm.SubmissionDate != default)
                    {
                        vm.CreateDate = vm.SubmissionDate.ToLocalTime();
                    }
                    else
                    {
                        vm.CreateDate = vm.JoinDate.ToLocalTime();
                    }

                    vm.GameJam = gameJamVm;
                }

                vmList = vmList.OrderBy(x => x.SubmissionDate).ToList();

                return new OperationResultListVo<GameJamEntryViewModel>(vmList);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameJamEntryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<GameJamEntryViewModel>> GetParticipantsByJam(Guid currentUserId, bool currentUserIsAdmin, string jamHandler, Guid jamId)
        {
            try
            {
                List<GameJamEntryViewModel> finalList = new List<GameJamEntryViewModel>();

                IEnumerable<GameJamEntry> allModels = await mediator.Query<GetGameJamEntryListQuery, IEnumerable<GameJamEntry>>(new GetGameJamEntryListQuery(jamId, false));

                IEnumerable<Guid> userIds = allModels.Select(x => x.UserId);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIds));

                foreach (GameJamEntry entry in allModels)
                {
                    UserProfileEssentialVo profile = userProfiles.FirstOrDefault(x => x.UserId == entry.UserId);
                    if (profile != null)
                    {
                        GameJamEntryViewModel entryVm = new GameJamEntryViewModel
                        {
                            Id = entry.Id,
                            UserId = entry.UserId,
                            JoinDate = entry.JoinDate,
                            SubmissionDate = entry.SubmissionDate,
                            JamHandler = jamHandler,
                            CreateDate = profile.CreateDate,
                            Handler = profile.Handler,
                            Name = profile.Name,
                            Location = profile.Location,
                            ProfileImageUrl = UrlFormatter.ProfileImage(profile.UserId, Constants.HugeAvatarSize),
                            CoverImageUrl = UrlFormatter.ProfileCoverImage(profile.UserId, profile.Id, profile.LastUpdateDate, profile.HasCoverImage, Constants.ProfileCoverSize)
                        };

                        finalList.Add(entryVm);
                    }
                }

                finalList = finalList.OrderBy(x => x.JoinDate).ToList();

                return new OperationResultListVo<GameJamEntryViewModel>(finalList);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameJamEntryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<GameJamEntryViewModel>> GetWinnersByJam(Guid currentUserId, bool currentUserIsAdmin, Guid jamId, string jamHandler, int winnerCount)
        {
            try
            {
                GameJam gameJam = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(jamId));
                GameJamViewModel gameJamVm = mapper.Map<GameJamViewModel>(gameJam);

                SetGameJamState(DateTime.Now.ToLocalTime(), gameJamVm);

                IEnumerable<GameJamEntry> allModels = await mediator.Query<GetGameJamWinnersQuery, IEnumerable<GameJamEntry>>(new GetGameJamWinnersQuery(jamId, winnerCount));

                IEnumerable<GameJamEntryViewModel> vms = mapper.Map<IEnumerable<GameJamEntry>, IEnumerable<GameJamEntryViewModel>>(allModels);

                List<GameJamEntryViewModel> vmList = vms.ToList();

                IEnumerable<Guid> gamesIds = vms.Where(x => x.GameId != Guid.Empty).Select(x => x.GameId);

                IEnumerable<Game> games = await mediator.Query<GetGamesByIdsQuery, IEnumerable<Game>>(new GetGamesByIdsQuery(gamesIds));

                IEnumerable<Guid> userIds = vmList.Select(x => x.UserId);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIds));

                foreach (GameJamEntryViewModel vm in vmList)
                {
                    if (vm.GameId != Guid.Empty)
                    {
                        Game relatedGame = games.FirstOrDefault(x => x.Id == vm.GameId);
                        if (relatedGame != null)
                        {
                            vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(relatedGame.UserId, relatedGame.ThumbnailUrl, ImageRenderType.Full);
                        }
                    }
                    else
                    {
                        vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, Constants.DefaultGameThumbnail, ImageRenderType.Full);
                    }

                    UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == vm.UserId);
                    if (authorProfile != null)
                    {
                        vm.UserHandler = authorProfile.Handler;
                        vm.AuthorName = authorProfile.Name;
                        vm.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, Constants.SmallAvatarSize);
                    }

                    SetVotes(currentUserId, gameJamVm, vm);

                    if (vm.SubmissionDate != default)
                    {
                        vm.CreateDate = vm.SubmissionDate.ToLocalTime();
                    }
                    else
                    {
                        vm.CreateDate = vm.JoinDate.ToLocalTime();
                    }

                    vm.GameJam = gameJamVm;
                    vm.JamHandler = jamHandler;
                }

                vmList = vmList.OrderBy(x => x.FinalPlace).ToList();

                return new OperationResultListVo<GameJamEntryViewModel>(vmList);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GameJamEntryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Join(Guid currentUserId, Guid jamId)
        {
            try
            {
                GameJam model = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(jamId));

                if (model == null)
                {
                    return new OperationResultVo<GameJamViewModel>("Jam not found!");
                }

                CommandResult result = await mediator.SendCommand(new JoinGameJamCommand(currentUserId, jamId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, result.PointsEarned, "You are in!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> SaveTeam(Guid currentUserId, Guid entryId, IEnumerable<GameJamTeamMemberViewModel> teamMembers)
        {
            try
            {
                GameJamEntry entry = await mediator.Query<GetGameJamEntryByIdQuery, GameJamEntry>(new GetGameJamEntryByIdQuery(entryId));

                if (entry == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Entry not found!");
                }

                IEnumerable<Guid> teamMembersIds = teamMembers.Select(x => x.UserId);

                CommandResult result = await mediator.SendCommand(new SaveGameJamEntryTeamCommand(entryId, teamMembersIds));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(entry.Id, false, message);
                }

                return new OperationResultVo<Guid>(entry.Id, result.PointsEarned, "Team saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> SubmitGame(Guid currentUserId, string jamHandler, Guid gameId, IEnumerable<GameJamTeamMemberViewModel> teamMembers)
        {
            try
            {
                GameJam gameJam = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(jamHandler));

                if (gameJam == null)
                {
                    return new OperationResultVo<GameJamViewModel>("Jam not found!");
                }

                GameJamEntry entry = await mediator.Query<GetGameJamEntryQuery, GameJamEntry>(new GetGameJamEntryQuery(currentUserId, gameJam.Id));

                if (entry == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Entry not found!");
                }

                IEnumerable<Guid> teamMembersIds = teamMembers.Select(x => x.UserId);

                CommandResult result = await mediator.SendCommand(new SubmitGameJamEntryCommand(currentUserId, entry.Id, gameId, teamMembersIds));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(entry.Id, false, message);
                }

                return new OperationResultVo<Guid>(entry.Id, result.PointsEarned, "Game Submitted! Good luck!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> VoteEntry(Guid currentUserId, string jamHandler, Guid entryId, GameJamCriteriaType criteriaType, decimal score, string comment, bool isCommunityVote)
        {
            try
            {
                GameJam gameJam = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(jamHandler));

                if (gameJam == null)
                {
                    return new OperationResultVo<GameJamViewModel>("Jam not found!");
                }

                GameJamEntry entry = await mediator.Query<GetGameJamEntryQuery, GameJamEntry>(new GetGameJamEntryQuery(currentUserId, gameJam.Id));

                if (entry == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Entry not found!");
                }

                CommandResult result = await mediator.SendCommand(new VoteGameJamEntryCommand(currentUserId, entryId, criteriaType, score, comment, isCommunityVote));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(entry.Id, false, message);
                }

                return new OperationResultVo<Guid>(entry.Id, result.PointsEarned, "Vote cast");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameJamEntryViewModel>> GetEntry(Guid currentUserId, bool currentUserIsAdmin, string jamHandler)
        {
            return await GetEntry(currentUserId, currentUserIsAdmin, jamHandler, null);
        }

        public async Task<OperationResultVo<GameJamEntryViewModel>> GetEntry(Guid currentUserId, bool currentUserIsAdmin, string jamHandler, Guid? id)
        {
            try
            {
                GameJam gameJam = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(Guid.Empty, jamHandler));

                if (gameJam == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Jam not found!");
                }

                GameJamEntry model;

                if (id.HasValue)
                {
                    model = await mediator.Query<GetGameJamEntryByIdQuery, GameJamEntry>(new GetGameJamEntryByIdQuery(id.Value));
                }
                else
                {
                    model = await mediator.Query<GetGameJamEntryQuery, GameJamEntry>(new GetGameJamEntryQuery(currentUserId, gameJam.Id));
                }

                if (model == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Entry not found!");
                }

                GameJamViewModel gameJamVm = mapper.Map<GameJamViewModel>(gameJam);
                GameJamEntryViewModel vm = mapper.Map<GameJamEntryViewModel>(model);

                if (vm.GameId != Guid.Empty)
                {
                    Game game = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(vm.GameId));

                    GameViewModel gameVm = mapper.Map<GameViewModel>(game);

                    GameFormatter.FilCharacteristics(gameVm);

                    await GameFormatter.FormatExternalLinks(mediator, gameVm);

                    gameVm.ThumbnailUrl = string.IsNullOrWhiteSpace(gameVm.ThumbnailUrl) || Constants.DefaultGameThumbnail.NoExtension().Contains(gameVm.ThumbnailUrl.NoExtension()) ? Constants.DefaultGameThumbnail : UrlFormatter.Image(gameVm.UserId, ImageType.GameThumbnail, gameVm.ThumbnailUrl);

                    vm.Game = gameVm;
                    vm.Title = game.Title;

                    vm.FeaturedImage = gameVm.ThumbnailUrl;
                }

                SetGameJamImagesToShow(gameJamVm, false);
                vm.GameJam = gameJamVm;

                UserProfileEssentialVo authorProfile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(model.UserId));

                vm.AuthorName = authorProfile.Name;
                vm.UserHandler = authorProfile.Handler;
                vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId, Constants.BigAvatarSize);

                SetGameJamState(DateTime.Now.ToLocalTime(), gameJamVm);
                vm.SecondsToCountDown = gameJamVm.SecondsToCountDown;

                await SetTeamMembers(vm);

                vm.JoinDate = vm.JoinDate.ToLocalTime();
                vm.SubmissionDate = vm.SubmissionDate.ToLocalTime();

                SetVotes(currentUserId, gameJamVm, vm);

                vm.IsWinner = vm.FinalPlace <= gameJamVm.Winners;

                IEnumerable<GameJamTeamMember> allParticipantIds = await mediator.Query<GetGameJamParticipants, IEnumerable<GameJamTeamMember>>(new GetGameJamParticipants(gameJamVm.Id));

                SetEntryPermissions(currentUserId, currentUserIsAdmin, gameJamVm, vm, allParticipantIds);

                return new OperationResultVo<GameJamEntryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameJamEntryViewModel>(ex.Message);
            }
        }

        private static void SetVotes(Guid currentUserId, GameJamViewModel gameJamVm, GameJamEntryViewModel vm)
        {
            List<decimal> medians = new List<decimal>();

            foreach (GameJamCriteriaViewModel criteria in gameJamVm.Criteria)
            {
                decimal median = 0;

                IEnumerable<GameJamVoteViewModel> allVotesForThisCategory = vm.Votes.Where(x => x.CriteriaType == criteria.Type);
                if (allVotesForThisCategory.Any())
                {
                    median = allVotesForThisCategory.Median(x => x.Score);
                }

                GameJamVoteViewModel newVote = new GameJamVoteViewModel
                {
                    UserId = currentUserId,
                    CriteriaType = criteria.Type,
                    Median = median
                };

                if (currentUserId == Guid.Empty)
                {
                    vm.Votes.Add(newVote);
                }
                else
                {
                    GameJamVoteViewModel currentUserVote = allVotesForThisCategory.FirstOrDefault(x => x.UserId == currentUserId);
                    if (currentUserVote == null)
                    {
                        vm.Votes.Add(newVote);
                    }
                    else
                    {
                        currentUserVote.Median = median;
                    }
                }

                medians.Add(median);
            }

            vm.TotalScore = medians.Any() ? medians.Median() : 0;

            vm.CanShowFinalResults = gameJamVm.CurrentPhase == GameJamPhase.Finished;
            vm.CanShowResults = vm.CanShowFinalResults || gameJamVm.CurrentPhase == GameJamPhase.Results || (gameJamVm.CurrentPhase == GameJamPhase.Voting && !gameJamVm.HideRealtimeResults);

            vm.IsOverallVote = !gameJamVm.Criteria.Any(x => x.Type != GameJamCriteriaType.Overall);
        }

        private static void SetViewModelState(Guid currentUserId, GameJamViewModel vm, IEnumerable<Guid> entries)
        {
            vm.JoinCount = entries.Count();
            vm.CurrentUserJoined = entries.Any(x => x == currentUserId);
            vm.ShowMainTheme =
                (vm.CurrentPhase == GameJamPhase.Warmup && !vm.HideMainTheme && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Submission && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Voting && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Results && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Finished);
        }

        private static void SetViewModelStates(Guid currentUserId, bool currentUserIsAdmin, IEnumerable<GameJamViewModel> vms)
        {
            DateTime localTime = DateTime.Now.ToLocalTime();
            foreach (GameJamViewModel vm in vms)
            {
                SetDates(vm);

                SetGameJamState(localTime, vm);

                vm.FeaturedImage = SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Small, Constants.DefaultGamejamThumbnail);

                SetJamPermissions(currentUserId, currentUserIsAdmin, vm);
            }
        }

        private static void SetDates(GameJamViewModel vm)
        {
            if (vm.StartDate == default)
            {
                vm.StartDate = vm.CreateDate.AddDays(7);
            }
            if (vm.EntryDeadline == default)
            {
                vm.EntryDeadline = vm.StartDate.AddDays(7);
            }
            if (vm.VotingEndDate == default)
            {
                vm.VotingEndDate = vm.EntryDeadline.AddDays(7);
            }
            if (vm.ResultDate == default)
            {
                vm.ResultDate = vm.VotingEndDate.AddDays(7);
            }

            vm.CreateDate = vm.CreateDate.ToLocalTime();

            vm.StartDate = vm.StartDate.ToLocalTime();
            vm.EntryDeadline = vm.EntryDeadline.ToLocalTime();
            vm.VotingEndDate = vm.VotingEndDate.ToLocalTime();
            vm.ResultDate = vm.ResultDate.ToLocalTime();
        }

        private static void SetGameJamState(DateTime localTime, GameJamViewModel vm)
        {
            TimeSpan diff;
            if (vm.ResultDate <= localTime)
            {
                vm.CurrentPhase = GameJamPhase.Finished;
                vm.CountDownMessage = "We did it!";
                diff = new TimeSpan();
            }
            else if (vm.VotingEndDate <= localTime && vm.ResultDate > localTime)
            {
                vm.CurrentPhase = GameJamPhase.Results;
                vm.CountDownMessage = "Results in";
                diff = vm.ResultDate - localTime;
            }
            else if (vm.EntryDeadline <= localTime && vm.VotingEndDate > localTime)
            {
                vm.CurrentPhase = GameJamPhase.Voting;
                vm.CountDownMessage = "Voting";
                diff = vm.VotingEndDate - localTime;
            }
            else if (vm.StartDate <= localTime && vm.EntryDeadline > localTime)
            {
                vm.CurrentPhase = GameJamPhase.Submission;
                vm.CountDownMessage = "Deadline";
                diff = vm.EntryDeadline - localTime;
            }
            else
            {
                vm.CurrentPhase = GameJamPhase.Warmup;
                vm.CountDownMessage = "Starts in";
                diff = vm.StartDate - localTime;
            }
            vm.SecondsToCountDown = (int)diff.TotalSeconds;
        }

        private static void SanitizeHtml(GameJamViewModel viewModel, HtmlSanitizer sanitizer)
        {
            viewModel.Description = sanitizer.Sanitize(viewModel.Description, Constants.DefaultLuduStackPath);

            foreach (string key in ContentFormatter.Replacements().Keys)
            {
                viewModel.Description = viewModel.Description.Replace(key, ContentFormatter.Replacements()[key]);
            }
        }

        private async Task SetJudges(GameJamViewModel vm)
        {
            IEnumerable<Guid> judgesIds = vm.Judges.Select(x => x.UserId);

            IEnumerable<UserProfileEssentialVo> judgesProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(judgesIds));

            vm.JudgesProfiles = new List<ProfileViewModel>();
            foreach (UserProfileEssentialVo profileEssential in judgesProfiles)
            {
                ProfileViewModel profile = new ProfileViewModel
                {
                    UserId = profileEssential.UserId,
                    Handler = profileEssential.Handler,
                    Location = profileEssential.Location,
                    CreateDate = profileEssential.CreateDate,
                    Name = profileEssential.Name,
                    ProfileImageUrl = UrlFormatter.ProfileImage(profileEssential.UserId, Constants.HugeAvatarSize),
                    CoverImageUrl = UrlFormatter.ProfileCoverImage(profileEssential.UserId, profileEssential.Id, profileEssential.LastUpdateDate, profileEssential.HasCoverImage, Constants.ProfileCoverSize)
                };

                vm.JudgesProfiles.Add(profile);
            }
        }

        private async Task SetTeamMembers(GameJamEntryViewModel vm)
        {
            IEnumerable<Guid> teamMembersIds = vm.TeamMembers.Select(x => x.UserId);

            IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(teamMembersIds));

            vm.TeamMembersProfiles = new List<ProfileViewModel>();
            foreach (UserProfileEssentialVo profileEssential in profiles)
            {
                GameJamTeamMemberViewModel teamMember = vm.TeamMembers.First(x => x.UserId == profileEssential.UserId);

                ProfileViewModel profile = new ProfileViewModel
                {
                    UserId = profileEssential.UserId,
                    Handler = profileEssential.Handler,
                    Location = profileEssential.Location,
                    CreateDate = profileEssential.CreateDate,
                    LastUpdateDate = teamMember.TeamJoinDate,
                    Name = profileEssential.Name,
                    ProfileImageUrl = UrlFormatter.ProfileImage(profileEssential.UserId, Constants.HugeAvatarSize),
                    CoverImageUrl = UrlFormatter.ProfileCoverImage(profileEssential.UserId, profileEssential.Id, profileEssential.LastUpdateDate, profileEssential.HasCoverImage, Constants.ProfileCoverSize)
                };

                vm.TeamMembersProfiles.Add(profile);
            }

            vm.TeamMembersProfiles = vm.TeamMembersProfiles.OrderByDescending(x => x.LastUpdateDate).ToList();
        }

        private static void SetCriteria(GameJamViewModel gameJamVm, bool isNew)
        {
            if (gameJamVm.Criteria == null)
            {
                gameJamVm.Criteria = new List<GameJamCriteriaViewModel>();
            }

            IEnumerable<GameJamCriteriaType> allCriteria = Enum.GetValues(typeof(GameJamCriteriaType)).Cast<GameJamCriteriaType>();

            foreach (GameJamCriteriaType item in allCriteria)
            {
                UiInfoAttribute uiInfo = item.ToUiInfo();

                GameJamCriteriaViewModel existingCriteria = gameJamVm.Criteria.FirstOrDefault(x => x.Type == item);
                if (existingCriteria != null)
                {
                    existingCriteria.Enabled = true;

                    if (string.IsNullOrWhiteSpace(existingCriteria.Name))
                    {
                        existingCriteria.Name = uiInfo.Display;
                    }
                }
                else
                {
                    GameJamCriteriaViewModel newCriteria = new GameJamCriteriaViewModel
                    {
                        Enabled = isNew && ((int)item) < 5,
                        Type = item,
                        Name = uiInfo.Display,
                        Weight = 1
                    };

                    gameJamVm.Criteria.Add(newCriteria);
                }
            }

            gameJamVm.Criteria = gameJamVm.Criteria.OrderBy(x => x.Type).ToList();
        }

        private static void SetJamPermissions(Guid currentUserId, bool currentUserIsAdmin, GameJamViewModel vm)
        {
            bool canJoinOnWarming = vm.CurrentPhase == GameJamPhase.Warmup;
            bool canJoinLate = vm.CurrentPhase == GameJamPhase.Submission && vm.AllowLateJoin;
            bool sameUser = vm.UserId == currentUserId;
            bool iAmJudge = vm.Judges != null && vm.Judges.Select(x => x.UserId).Contains(currentUserId);
            bool isNotWarmup = vm.CurrentPhase != GameJamPhase.Warmup;

            vm.Permissions.CanJoin = !sameUser && (canJoinOnWarming || canJoinLate);

            if (!vm.Permissions.CanJoin)
            {
                vm.CantJoinMessage = sameUser ? "You can't join your own Game Jam!" : "You can't join anymore!";
            }

            vm.ShowSubmissions = (isNotWarmup && iAmJudge) || (isNotWarmup && !vm.HideSubmissions || vm.CurrentPhase == GameJamPhase.Results || vm.CurrentPhase == GameJamPhase.Finished);
            vm.ShowJudges = vm.Judges != null && vm.Judges.Any();
            vm.ShowCriteria = vm.Criteria != null && vm.Criteria.Where(x => x.Type != GameJamCriteriaType.Overall).Any();
            vm.ShowFinalResults = vm.CurrentPhase == GameJamPhase.Finished && vm.HasWinners;

            vm.Permissions.IsAdmin = currentUserIsAdmin;
            vm.Permissions.CanDelete = vm.Permissions.IsAdmin;
            vm.Permissions.CanSubmit = vm.CurrentPhase == GameJamPhase.Submission;
        }

        private static void SetEntryPermissions(Guid currentUserId, bool currentUserIsAdmin, GameJamViewModel gameJamVm, GameJamEntryViewModel vm, IEnumerable<GameJamTeamMember> allParticipantIds)
        {
            SetJamPermissions(currentUserId, currentUserIsAdmin, gameJamVm);

            vm.Permissions.IsMe = currentUserId == vm.UserId || vm.TeamMembers.Any(x => x.UserId == currentUserId);
            vm.Permissions.IsAdmin = currentUserIsAdmin;
            vm.Permissions.CanSubmit = vm.Permissions.IsMe && (gameJamVm.Permissions.CanSubmit || (gameJamVm.CurrentPhase == GameJamPhase.Voting && vm.LateSubmission));

            bool iAmJudge = gameJamVm.Judges != null && gameJamVm.Judges.Select(x => x.UserId).Any(x => x == currentUserId);
            bool jamPhaseAllowsVote = gameJamVm.CurrentPhase == GameJamPhase.Voting;
            bool iCanVote = iAmJudge;

            switch (gameJamVm.Voters)
            {
                case GameJamVoters.JudgesAndSubmitters:
                    iCanVote = iCanVote || allParticipantIds.Any(x => x.IsSubmitter && x.UserId == currentUserId);
                    break;
                case GameJamVoters.JudgesAndTeamMembers:
                    iCanVote = iCanVote || allParticipantIds.Any(x => x.UserId == currentUserId);
                    break;
                case GameJamVoters.JudgesAndNonParticipants:
                    iCanVote = (iCanVote || !allParticipantIds.Any(x => x.UserId == currentUserId)) && currentUserId != Guid.Empty;
                    break;
                case GameJamVoters.JudgesAndTheWholeCommunity:
                    iCanVote = iCanVote || currentUserId != Guid.Empty;
                    break;
            }

            vm.Permissions.CanVote = !vm.Permissions.IsMe && jamPhaseAllowsVote && vm.GameId != Guid.Empty && iCanVote;
        }

        private void SetGameJamImagesToShow(GameJamViewModel vm, bool editMode)
        {
            if (editMode)
            {
                if (string.IsNullOrWhiteSpace(vm.FeaturedImage))
                {
                    vm.FeaturedImage = Constants.DefaultGamejamThumbnail;
                }

                if (string.IsNullOrWhiteSpace(vm.BannerImage))
                {
                    vm.BannerImage = Constants.DefaultGamejamThumbnail;
                }

                if (string.IsNullOrWhiteSpace(vm.BackgroundImage))
                {
                    vm.BackgroundImage = Constants.DefaultGamejamThumbnail;
                }
            }

            vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, vm.FeaturedImage, ImageRenderType.Full);
            vm.BannerImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, vm.BannerImage, ImageRenderType.Full);
            vm.BackgroundImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, vm.BackgroundImage, ImageRenderType.Full);

            if (string.IsNullOrWhiteSpace(vm.BackgroundColor))
            {
                vm.BackgroundColor = "#dedada";
            }
        }

        private static void SetHighlights(GameJamViewModel vm)
        {
            vm.Highlights = new List<GameJamHighlightsVo>();
            if (vm.HideSubmissions)
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.SubmissionsSecret });
            }
            else
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.SubmissionsPublic });
            }

            if (vm.HideRealtimeResults)
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.ResultsSecret });
            }
            else
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.ResultsPublic });
            }

            if (vm.Unlisted)
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.Unlisted });
            }

            if (vm.HideMainTheme)
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.MainThemeSecret });
            }

            if (!vm.AllowLateJoin)
            {
                vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.LateJoinForbidden });
            }

            switch (vm.ParticipationType)
            {
                case GameJamParticipationType.IndividualsOnly:
                    vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.IndividualsOnly });
                    break;
                case GameJamParticipationType.TeamsOnly:
                    vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.TeamsOnly });
                    break;
                case GameJamParticipationType.IndividualsAndTeams:
                    vm.Highlights.Add(new GameJamHighlightsVo { Highlight = GameJamHighlight.IndividualsAndTeams });
                    break;
            }
        }
    }
}