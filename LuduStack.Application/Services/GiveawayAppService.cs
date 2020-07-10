using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.Services
{
    public class GiveawayAppService : ProfileBaseAppService, IGiveawayAppService
    {
        private readonly IGiveawayDomainService giveawayDomainService;
        private readonly IGamificationDomainService gamificationDomainService;

        public GiveawayAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , IGiveawayDomainService giveawayDomainService
            , IGamificationDomainService gamificationDomainService) : base(profileBaseAppServiceCommon)
        {
            this.giveawayDomainService = giveawayDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        public OperationResultVo GenerateNew(Guid currentUserId)
        {
            try
            {
                Giveaway model = giveawayDomainService.GenerateNewGiveaway(currentUserId);

                GiveawayViewModel newVm = mapper.Map<GiveawayViewModel>(model);

                return new OperationResultVo<GiveawayViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetGiveawayBasicInfoById(Guid currentUserId, Guid id)
        {
            try
            {
                GiveawayBasicInfo existing = giveawayDomainService.GetGiveawayBasicInfoById(id);

                GiveawayViewModel vm = mapper.Map<GiveawayViewModel>(existing);

                SetAuthorDetails(vm);

                SetViewModelState(currentUserId, vm);

                vm.FeaturedImage = SetFeaturedImage(currentUserId, vm.FeaturedImage, ImageRenderType.Full, Constants.DefaultGiveawayThumbnail);

                return new OperationResultVo<GiveawayViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetGiveawayParticipantInfo(Guid currentUserId, Guid giveawayId, string email)
        {
            try
            {
                GiveawayBasicInfo existing = giveawayDomainService.GetGiveawayBasicInfoById(giveawayId);

                if (existing == null)
                {
                    return new OperationResultVo(false, "Giveaway not found");
                }

                GiveawayParticipant participant = giveawayDomainService.GetParticipantByEmail(giveawayId, email);

                if (participant == null)
                {
                    return new OperationResultVo(false, "No participant found for that email");
                }

                GiveawayParticipationViewModel vm = mapper.Map<GiveawayParticipationViewModel>(existing);

                vm.EntryCount = participant.Entries.Count;

                vm.ShareUrl = participant.ReferalCode;

                SetViewModelState(currentUserId, vm);

                vm.FeaturedImage = SetFeaturedImage(currentUserId, vm.FeaturedImage, ImageRenderType.Full, Constants.DefaultGiveawayThumbnail);

                return new OperationResultVo<GiveawayParticipationViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetGiveawaysByMe(Guid currentUserId)
        {
            try
            {
                List<GiveawayListItemVo> courses = giveawayDomainService.GetGiveawayListByUserId(currentUserId);

                return new OperationResultListVo<GiveawayListItemVo>(courses);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo<Guid> SaveGiveaway(Guid currentUserId, GiveawayViewModel vm)
        {
            int pointsEarned = 0;

            try
            {
                Giveaway model;

                Giveaway existing = giveawayDomainService.GetGiveawayById(vm.Id);
                if (existing != null)
                {
                    model = mapper.Map(vm, existing);
                }
                else
                {
                    model = mapper.Map<Giveaway>(vm);
                }

                if (vm.Id == Guid.Empty)
                {
                    giveawayDomainService.Add(model);
                    vm.Id = model.Id;

                    pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.GiveawayAdd);
                }
                else
                {
                    giveawayDomainService.Update(model);
                }

                unitOfWork.Commit();

                vm.Id = model.Id;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public OperationResultVo RemoveGiveaway(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                giveawayDomainService.Remove(id);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Giveaway is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm)
        {
            try
            {
                giveawayDomainService.AddParticipant(vm.GiveawayId, vm.Email, vm.GdprConsent, vm.WantNotifications);

                unitOfWork.Commit();

                return new OperationResultVo(true, "You are in!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void SetViewModelState(Guid currentUserId, IGiveawayScreenViewModel vm)
        {
            vm.Permissions.CanConnect = vm.UserId != currentUserId;

            vm.StatusMessage = vm.Status.ToDisplayName();

            if (vm.Status == GiveawayStatus.PendingStart && vm.StartDate <= DateTime.Now)
            {
                vm.Status = GiveawayStatus.OpenForEntries;
                vm.StatusMessage = "enter your email address below";
            }

            if (vm.EndDate.HasValue)
            {
                TimeSpan diff;

                if (vm.StartDate >= DateTime.Now)
                {
                    diff = (vm.StartDate - DateTime.Now);
                    vm.Future = true;
                }
                else
                {
                    diff = (vm.EndDate - DateTime.Now).Value;

                    if (DateTime.Now >= vm.EndDate.Value)
                    {
                        vm.Status = GiveawayStatus.PickingWinners;
                        vm.StatusMessage = "we are picking winners";
                    }
                }

                vm.SecondsToEnd = (int)diff.TotalSeconds;
            }
            else
            {
                vm.StatusMessage = "this giveaway was not started yet";
            }

            vm.CanCountDown = vm.EndDate.HasValue && (vm.Status == GiveawayStatus.Draft || vm.Status == GiveawayStatus.OpenForEntries);

            vm.ShowTimeZone = !string.IsNullOrWhiteSpace(vm.TimeZone);
            vm.ShowSponsor = !string.IsNullOrWhiteSpace(vm.SponsorName);
            vm.SponsorWebsite = string.IsNullOrWhiteSpace(vm.SponsorWebsite) ? "#" : vm.SponsorWebsite;

            SetBasePermissions(currentUserId, vm);
        }
    }
}