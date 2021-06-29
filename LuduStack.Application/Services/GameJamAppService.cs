﻿using Ganss.XSS;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
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

                return new OperationResultListVo<GameJamViewModel>(vms.OrderBy(x => x.Name));
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

                vm.AuthorName = authorProfile.Name;
                vm.AuthorHandler = authorProfile.Handler;

                DateTime localTime = DateTime.Now.ToLocalTime();
                SetCountdown(localTime, vm);

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();

                SanitizeHtml(vm, sanitizer);

                SetPermissions(currentUserId, currentUserIsAdmin, vm);

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

                var userCanEdit = model.UserId == currentUserId || currentUserIsAdmin;

                if (!userCanEdit)
                {
                    return new OperationResultVo<GameJamViewModel>("You cannot edit this!");
                }

                GameJamViewModel vm = mapper.Map<GameJamViewModel>(model);

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
                    StartDate = DateTime.Now.AddDays(7)
                };
                newVm.EntryDeadline = newVm.StartDate.AddDays(7);
                newVm.VotingEndDate = newVm.EntryDeadline.AddDays(7);
                newVm.ResultDate = newVm.VotingEndDate.AddDays(7);

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
                vm.CurrentPhase = GameJamPhase.Warming;
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
            bool canJoinOnWarming = vm.CurrentPhase == GameJamPhase.Warming;
            bool canJoinLate = vm.CurrentPhase == GameJamPhase.Submission && vm.AllowLateJoin;

            vm.Permissions.CanJoin = vm.UserId != currentUserId && (canJoinOnWarming || canJoinLate);
            vm.Permissions.IsAdmin = currentUserIsAdmin;
            vm.Permissions.CanDelete = vm.Permissions.IsAdmin;
        }
    }
}