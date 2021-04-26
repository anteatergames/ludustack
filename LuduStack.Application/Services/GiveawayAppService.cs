using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Giveaway;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GiveawayAppService : ProfileBaseAppService, IGiveawayAppService
    {
        private readonly IGiveawayDomainService giveawayDomainService;

        public GiveawayAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon, IGiveawayDomainService giveawayDomainService) : base(profileBaseAppServiceCommon)
        {
            this.giveawayDomainService = giveawayDomainService;
        }

        public OperationResultVo GenerateNew(Guid currentUserId)
        {
            try
            {
                Giveaway model = giveawayDomainService.GenerateNewGiveaway(currentUserId);

                GiveawayViewModel newVm = mapper.Map<GiveawayViewModel>(model);

                SetImagesToShow(newVm, true);

                return new OperationResultVo<GiveawayViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetGiveawayForManagement(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                Giveaway existing = await mediator.Query<GetGiveawayByIdQuery, Giveaway>(new GetGiveawayByIdQuery(giveawayId));

                GiveawayViewModel vm = mapper.Map<GiveawayViewModel>(existing);

                await SetAuthorDetails(currentUserId, vm);

                SetViewModelState(currentUserId, vm);

                SetImagesToShow(vm, false);

                return new OperationResultVo<GiveawayViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                GiveawayBasicInfo existing = giveawayDomainService.GetGiveawayBasicInfoById(giveawayId);

                GiveawayViewModel vm = mapper.Map<GiveawayViewModel>(existing);

                await SetAuthorDetails(currentUserId, vm);

                SetViewModelState(currentUserId, vm);

                SetImagesToShow(vm, true);

                return new OperationResultVo<GiveawayViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetForDetails(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                GiveawayBasicInfo existing = giveawayDomainService.GetGiveawayBasicInfoById(giveawayId);

                GiveawayViewModel vm = mapper.Map<GiveawayViewModel>(existing);

                await SetAuthorDetails(currentUserId, vm);

                SetViewModelState(currentUserId, vm);

                SetImagesToShow(vm, false);

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
                List<GiveawayListItemVo> giveaways = giveawayDomainService.GetGiveawayListByUserId(currentUserId);

                return new OperationResultListVo<GiveawayListItemVo>(giveaways);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> SaveGiveaway(Guid currentUserId, GiveawayViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                Giveaway model;

                Giveaway existing = await mediator.Query<GetGiveawayByIdQuery, Giveaway>(new GetGiveawayByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Giveaway>(viewModel);
                }

                FormatImagesToSave(model);

                CommandResult result = await mediator.SendCommand(new SaveGiveawayCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> DeleteGiveaway(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                await mediator.SendCommand(new DeleteGiveawayCommand(currentUserId, giveawayId));

                return new OperationResultVo(true, "That Giveaway is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo DuplicateGiveaway(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                Giveaway newGiveaway = giveawayDomainService.Duplicate(giveawayId);

                unitOfWork.Commit();

                return new OperationResultVo<Guid>(newGiveaway.Id, 0, "Giveaway duplicated!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo CheckParticipant(Guid currentUserId, Guid giveawayId, string sessionEmail)
        {
            try
            {
                bool exists = giveawayDomainService.CheckParticipantByEmail(giveawayId, sessionEmail);

                if (!exists)
                {
                    return new OperationResultVo("No Participant found with that email");
                }

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase)
        {
            try
            {
                string myCode = Guid.NewGuid().NoHyphen();

                DomainOperationVo<GiveawayParticipant> domainActionPerformed = giveawayDomainService.AddParticipant(vm.GiveawayId, vm.Email, vm.GdprConsent, vm.WantNotifications, myCode, vm.ReferralCode, vm.EntryType);

                await unitOfWork.Commit();

                if (domainActionPerformed.Action == DomainActionPerformed.Create)
                {
                    string urlReferral = string.Format("{0}?referralCode={1}", urlReferralBase, myCode);

                    SaveShortUrlCommand saveShortUrlCommand = new SaveShortUrlCommand(urlReferral, ShortUrlDestinationType.Giveaway);

                    CommandResult result = await mediator.SendCommand(saveShortUrlCommand);

                    if (!result.Validation.IsValid)
                    {
                        string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                        return new OperationResultVo<string>(saveShortUrlCommand.ShortUrl.OriginalUrl, false, message);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(saveShortUrlCommand.ShortUrl.NewUrl))
                        {
                            giveawayDomainService.UpdateParticipantShortUrl(vm.GiveawayId, vm.Email, saveShortUrlCommand.ShortUrl.NewUrl);

                            await unitOfWork.Commit();
                        }

                        return new OperationResultVo<string>(myCode, 0, "You are in!");
                    }
                }

                return new OperationResultVo<string>(string.Empty, 0, "You are in!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo DailyEntry(Guid currentUserId, Guid giveawayId, Guid participantId)
        {
            try
            {
                DomainOperationVo<int> domainOperation = giveawayDomainService.DailyEntry(giveawayId, participantId);

                if (domainOperation.Action == DomainActionPerformed.None)
                {
                    return new OperationResultVo(false, "You already have one entry for today!<br>Come back tomorrow for more!");
                }

                unitOfWork.Commit();

                return new OperationResultVo<int>(domainOperation.Entity, "You got one more entry for today!<br>Come back tomorrow for more!");
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
                    return new OperationResultVo(false, "Giveaway not found!");
                }

                GiveawayParticipant participant = giveawayDomainService.GetParticipantByEmail(giveawayId, email);

                if (participant == null)
                {
                    return new OperationResultVo(false, "No participant found for that email!");
                }

                GiveawayParticipationViewModel vm = mapper.Map<GiveawayParticipationViewModel>(existing);

                SetViewModelState(currentUserId, vm);

                vm.EntryCount = participant.Entries.Sum(x => x.Points);

                vm.ShareUrl = participant.ShortUrl;

                vm.EmailConfirmed = participant.Entries.Any(x => x.Type == GiveawayEntryType.EmailConfirmed);

                SetImagesToShow(vm, false);

                SetEntryOptions(vm, participant);

                vm.ParticipantId = participant.Id;

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

        public OperationResultVo RemoveParticipant(Guid currentUserId, Guid giveawayId, Guid participantId)
        {
            try
            {
                giveawayDomainService.RemoveParticipant(giveawayId, participantId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Participant is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo ClearParticipants(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                giveawayDomainService.ClearParticipants(giveawayId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "All Participants are gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo PickSingleWinner(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                giveawayDomainService.PickSingleWinner(giveawayId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "A single winner has been chosen!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo PickAllWinners(Guid currentUserId, Guid giveawayId)
        {
            try
            {
                giveawayDomainService.PickAllWinners(giveawayId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "All winners were chosen!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo DeclareNotWinner(Guid currentUserId, Guid giveawayId, Guid participantId)
        {
            try
            {
                giveawayDomainService.DeclareNotWinner(giveawayId, participantId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Participant is not a winner anymore!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void SetViewModelState(Guid currentUserId, IGiveawayScreenViewModel vm)
        {
            TimeSpan diff = vm.EndDate.HasValue && vm.StartDate <= DateTime.Now.ToLocalTime() ? (vm.EndDate - DateTime.Now.ToLocalTime()).Value : (vm.StartDate - DateTime.Now.ToLocalTime());

            vm.SecondsToEnd = (int)diff.TotalSeconds;

            vm.CanCountDown = vm.SecondsToEnd > 0;

            switch (vm.Status)
            {
                case GiveawayStatus.PendingStart:
                    vm.Future = true;
                    vm.StatusMessage = "This giveaway was not started yet!";
                    break;

                case GiveawayStatus.PickingWinners:
                    vm.StatusMessage = "We are picking winners!";
                    break;

                case GiveawayStatus.Ended:
                    vm.StatusMessage = "Thank you for participating!";
                    break;

                default:
                    vm.StatusMessage = "Enter your email address below:";
                    break;
            }

            if (vm.EndDate.HasValue && vm.Status != GiveawayStatus.PickingWinners && vm.Status != GiveawayStatus.Ended)
            {
                vm.Future = vm.Status == GiveawayStatus.PendingStart;
            }

            vm.CanReceiveEntries = vm.Status == GiveawayStatus.OpenForEntries;

            vm.ShowTimeZone = !string.IsNullOrWhiteSpace(vm.TimeZone);
            vm.ShowSponsor = !string.IsNullOrWhiteSpace(vm.SponsorName);
            vm.SponsorWebsite = string.IsNullOrWhiteSpace(vm.SponsorWebsite) ? "#" : vm.SponsorWebsite;

            vm.Permissions.CanConnect = vm.UserId != currentUserId;
            SetBasePermissions(currentUserId, vm);
        }

        private static void SetImagesToShow(IGiveawayScreenViewModel vm, bool editing)
        {
            List<string> newList = new List<string>();

            List<string> originalList = vm.ImageList;
            int originalListCount = originalList != null && originalList.Any() ? originalList.Count : 1;

            int maxIterations = (editing ? 5 : originalListCount);

            for (int i = 0; i < maxIterations; i++)
            {
                if (originalList != null && originalList.Any() && i < originalListCount && originalList.ElementAt(i) != null)
                {
                    newList.Add(UrlFormatter.Image(vm.UserId, ImageType.FeaturedImage, originalList.ElementAt(i), 720, 0));
                }
                else
                {
                    newList.Add(Constants.DefaultGiveawayThumbnail);
                }
            }

            vm.ImageList = newList;

            bool firstIsPlaceholder = vm.ImageList?.First().Equals(Constants.DefaultGiveawayThumbnail) ?? false;

            if (!string.IsNullOrWhiteSpace(vm.FeaturedImage))
            {
                ReplaceImage(vm, editing);
            }
            else if (vm.ImageList != null && vm.ImageList.Any() && !firstIsPlaceholder)
            {
                vm.FeaturedImage = UrlFormatter.Image(vm.UserId, ImageType.FeaturedImage, vm.ImageList.First(), 720, 0);
            }
            else
            {
                vm.FeaturedImage = UrlFormatter.Image(vm.UserId, ImageType.FeaturedImage, Constants.DefaultGiveawayThumbnail, 720, 0);
            }
        }

        private static void ReplaceImage(IGiveawayScreenViewModel vm, bool editing)
        {
            if (!editing)
            {
                string imageInTheList = vm.ImageList.FirstOrDefault(x => x.Contains(vm.FeaturedImage));
                int index = vm.ImageList.IndexOf(imageInTheList);
                if (index >= 0)
                {
                    vm.ImageList.RemoveAt(index);
                    vm.ImageList.Insert(0, imageInTheList);
                }

                vm.FeaturedImage = UrlFormatter.Image(vm.UserId, ImageType.FeaturedImage, vm.FeaturedImage, 720, 0);
            }
        }

        private static void FormatImagesToSave(IGiveawayBasicInfo model)
        {
            List<string> newImageList = new List<string>();
            for (int i = 0; i < model.ImageList.Count; i++)
            {
                if (!model.ImageList.ElementAt(i).Contains(Constants.DefaultGiveawayThumbnail))
                {
                    string newValue = model.ImageList.ElementAt(i).Split('/').LastOrDefault();
                    newImageList.Add(newValue);
                }
            }
            model.ImageList = newImageList;

            if (!string.IsNullOrWhiteSpace(model.FeaturedImage) && !(model.FeaturedImage.Contains(Constants.DefaultGiveawayThumbnail) || Constants.DefaultGiveawayThumbnail.Contains(model.FeaturedImage)))
            {
                model.FeaturedImage = model.FeaturedImage.Split('/').LastOrDefault();
            }
            else
            {
                model.FeaturedImage = null;
            }
        }

        private void SetEntryOptions(GiveawayParticipationViewModel vm, GiveawayParticipant participant)
        {
            Dictionary<GiveawayEntryType, int> groupedEntries = participant.Entries.GroupBy(x => x.Type).Select(x => new { EntryType = x.Key, EntryCount = x.Sum(y => y.Points) }).ToDictionary(x => x.EntryType, y => y.EntryCount);

            List<GiveawayEntryType> entryOptionsList = Enum.GetValues(typeof(GiveawayEntryType)).Cast<GiveawayEntryType>().ToList();

            foreach (GiveawayEntryType entryType in entryOptionsList)
            {
                KeyValuePair<GiveawayEntryType, int> entrySum = groupedEntries.FirstOrDefault(x => x.Key == entryType);

                GiveawayEntryOptionViewModel existing = vm.EntryOptions.FirstOrDefault(x => x.Type == entryType);
                if (existing != null)
                {
                    existing.Type = entryType;
                    existing.Points = entrySum.Value;
                    existing.IsMandatory = IsOptionMandatory(participant, entrySum);
                }
                else
                {
                    vm.EntryOptions.Add(new GiveawayEntryOptionViewModel
                    {
                        Type = entryType,
                        Name = entryType.ToUiInfo().Description,
                        Points = entrySum.Value,
                        IsMandatory = IsOptionMandatory(participant, entrySum)
                    });
                }
            }
        }

        private bool IsOptionMandatory(GiveawayParticipant participant, KeyValuePair<GiveawayEntryType, int> entrySum)
        {
            switch (entrySum.Key)
            {
                case GiveawayEntryType.LoginOrEmail:
                    return true;

                case GiveawayEntryType.EmailConfirmed:
                    return entrySum.Value > 0;

                case GiveawayEntryType.ReferralCode:
                    return participant.Entries.Any(x => x.Type == GiveawayEntryType.ReferralCode);

                case GiveawayEntryType.Daily:
                    return participant.Entries.Any(x => x.Type == GiveawayEntryType.Daily && x.Date.ToLocalTime().Date == DateTime.Today.ToLocalTime().Date);

                case GiveawayEntryType.FacebookShare:
                    return participant.Entries.Any(x => x.Type == GiveawayEntryType.FacebookShare);

                case GiveawayEntryType.TwitterShare:
                    return participant.Entries.Any(x => x.Type == GiveawayEntryType.TwitterShare);

                default:
                    return false;
            }
        }
    }
}