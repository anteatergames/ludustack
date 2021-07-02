using Ganss.XSS;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.GameJam;
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

                return new OperationResultListVo<GameJamViewModel>(vms.OrderBy(x => x.Name));
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
                var entries = await mediator.Query<GetEntriesUserIdsQuery, IEnumerable<Guid>>(new GetEntriesUserIdsQuery(x => x.GameJamId == model.Id));

                vm.AuthorName = authorProfile.Name;
                vm.AuthorHandler = authorProfile.Handler;

                DateTime localTime = DateTime.Now.ToLocalTime();
                SetCountdown(localTime, vm);

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();

                SanitizeHtml(vm, sanitizer);

                SetImagesToShow(vm, false);

                SetPermissions(currentUserId, currentUserIsAdmin, vm);

                SetViewModelState(currentUserId, vm, entries);

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

                SetImagesToShow(vm, true);

                SetPermissions(currentUserId, currentUserIsAdmin, vm);

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
                    Guid existingUserId = existing.UserId;
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

        public Task<OperationResultVo<GameJamViewModel>> GenerateNew(Guid currentUserId)
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

                return Task.FromResult(new OperationResultVo<GameJamViewModel>(newVm));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OperationResultVo<GameJamViewModel>(ex.Message));
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

        public async Task<OperationResultVo<GameJamEntryViewModel>> GetEntry(Guid currentUserId, bool currentUserIsAdmin, string jamHandler)
        {
            try
            {
                GameJam gameJamModel = await mediator.Query<GetGameJamByIdQuery, GameJam>(new GetGameJamByIdQuery(Guid.Empty, jamHandler));

                if (gameJamModel == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Jam not found!");
                }

                GameJamEntry model = await mediator.Query<GetGameJamEntryQuery, GameJamEntry>(new GetGameJamEntryQuery(currentUserId, gameJamModel.Id));

                if (model == null)
                {
                    return new OperationResultVo<GameJamEntryViewModel>("Entry not found!");
                }

                GameJamViewModel gameJamVm = mapper.Map<GameJamViewModel>(gameJamModel);
                GameJamEntryViewModel vm = mapper.Map<GameJamEntryViewModel>(model);

                if (vm.GameId != Guid.Empty)
                {
                    Game game = await mediator.Query<GetGameByIdQuery, Game>(new GetGameByIdQuery(vm.GameId));

                    GameViewModel gameVm = mapper.Map<GameViewModel>(game);

                    gameVm.ThumbnailUrl = string.IsNullOrWhiteSpace(gameVm.ThumbnailUrl) || Constants.DefaultGameThumbnail.NoExtension().Contains(gameVm.ThumbnailUrl.NoExtension()) ? Constants.DefaultGameThumbnail : UrlFormatter.Image(gameVm.UserId, ImageType.GameThumbnail, gameVm.ThumbnailUrl);
                    
                    vm.Game = gameVm;
                    vm.Title = game.Title;
                }
                else
                {
                    vm.Title = "Not Submited Yet";
                }

                SetImagesToShow(gameJamVm, false);
                vm.GameJam = gameJamVm;

                UserProfileEssentialVo authorProfile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(model.UserId));

                vm.AuthorName = authorProfile.Name;
                vm.UserHandler = authorProfile.Handler;

                return new OperationResultVo<GameJamEntryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameJamEntryViewModel>(ex.Message);
            }
        }

        private static void SetViewModelState(Guid currentUserId, GameJamViewModel vm, IEnumerable<Guid> entries)
        {
            vm.JoinCount = entries.Count();
            vm.CurrentUserJoined = entries.Any(x => x == currentUserId);
            vm.ShowMainTheme = !string.IsNullOrWhiteSpace(vm.MainTheme) && (
                (vm.CurrentPhase == GameJamPhase.Warmup && !vm.HideMainTheme && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Submission && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Voting && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Results && vm.CurrentUserJoined) ||
                (vm.CurrentPhase == GameJamPhase.Finished)
            );
        }

        private static void SetViewModelStates(Guid currentUserId, bool currentUserIsAdmin, IEnumerable<GameJamViewModel> vms)
        {
            DateTime localTime = DateTime.Now.ToLocalTime();
            foreach (GameJamViewModel vm in vms)
            {
                SetDates(vm);

                SetCountdown(localTime, vm);

                vm.FeaturedImage = SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Small, Constants.DefaultGamejamThumbnail);

                SetPermissions(currentUserId, currentUserIsAdmin, vm);
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

        private static void SetCountdown(DateTime localTime, GameJamViewModel vm)
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

        private static void SetPermissions(Guid currentUserId, bool currentUserIsAdmin, GameJamViewModel vm)
        {
            bool canJoinOnWarming = vm.CurrentPhase == GameJamPhase.Warmup;
            bool canJoinLate = vm.CurrentPhase == GameJamPhase.Submission && vm.AllowLateJoin;

            vm.Permissions.CanJoin = vm.UserId != currentUserId && (canJoinOnWarming || canJoinLate);
            vm.Permissions.IsAdmin = currentUserIsAdmin;
            vm.Permissions.CanDelete = vm.Permissions.IsAdmin;
        }

        private void SetImagesToShow(GameJamViewModel vm, bool editMode)
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
        }
    }
}