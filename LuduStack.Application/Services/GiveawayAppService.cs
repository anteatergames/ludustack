﻿using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.Services
{
    public class GiveawayAppService : ProfileBaseAppService, IGiveawayAppService
    {
        private readonly IGiveawayDomainService giveawayDomainService;
        private readonly IGamificationDomainService gamificationDomainService;
        private readonly IShortUrlDomainService shortUrlDomainService;

        public GiveawayAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , IGiveawayDomainService giveawayDomainService
            , IGamificationDomainService gamificationDomainService
            , IShortUrlDomainService shortUrlDomainService) : base(profileBaseAppServiceCommon)
        {
            this.giveawayDomainService = giveawayDomainService;
            this.gamificationDomainService = gamificationDomainService;
            this.shortUrlDomainService = shortUrlDomainService;
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

        public OperationResultVo GetGiveawayBasicInfoById(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                GiveawayBasicInfo existing = giveawayDomainService.GetGiveawayBasicInfoById(giveawayId);

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

        public OperationResultVo RemoveGiveaway(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                // validate before

                giveawayDomainService.Remove(giveawayId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Giveaway is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase)
        {///
            try
            {
                var newReferralCode = Guid.NewGuid().NoHyphen();

                var domainActionPerformed = giveawayDomainService.AddParticipant(vm.GiveawayId, vm.Email, vm.GdprConsent, vm.WantNotifications, newReferralCode, vm.ReferralCode);

                unitOfWork.Commit();

                if (domainActionPerformed.Action == DomainActionPerformed.Create)
                {
                    var urlReferral = string.Format("{0}?referralCode={1}", urlReferralBase, newReferralCode);

                    string shortUrl = shortUrlDomainService.Add(urlReferral);

                    if (!string.IsNullOrWhiteSpace(shortUrl))
                    {
                        giveawayDomainService.UpdateParticipantShortUrl(vm.GiveawayId, vm.Email, shortUrl);

                        unitOfWork.Commit();
                    }

                    return new OperationResultVo<string>(newReferralCode, 0, "You are in!");
                }

                return new OperationResultVo<string>(string.Empty, 0, "You are in!");

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

                SetViewModelState(currentUserId, vm);

                vm.EntryCount = participant.Entries.Sum(x => x.Points);

                vm.ShareUrl = participant.ShortUrl;

                vm.EmailConfirmed = participant.Entries.Any(x => x.Type == GiveawayEntryType.EmailConfirmed);

                vm.FeaturedImage = SetFeaturedImage(currentUserId, vm.FeaturedImage, ImageRenderType.Full, Constants.DefaultGiveawayThumbnail);

                return new OperationResultVo<GiveawayParticipationViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo ConfirmParticipant(Guid currentUserId, Guid giveawayId, string referralCode)
        {
            try
            {
                giveawayDomainService.ConfirmParticipant(giveawayId, referralCode);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Giveaway is gone now!");
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