using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class GiveawayDomainService : IGiveawayDomainService
    {
        protected readonly IGiveawayRepository giveawayRepository;

        public GiveawayDomainService(IGiveawayRepository giveawayRepository)
        {
            this.giveawayRepository = giveawayRepository;
        }

        public Giveaway GenerateNewGiveaway(Guid userId)
        {
            Giveaway model = new Giveaway
            {
                UserId = userId,
                Status = GiveawayStatus.Draft,
                StartDate = DateTime.Now.Date.AddDays(1)
            };

            return model;
        }

        public DomainOperationVo<int> DailyEntry(Guid giveawayId, Guid participantId)
        {
            GiveawayParticipant existing = giveawayRepository.GetParticipantById(giveawayId, participantId);
            if (existing == null)
            {
                return new DomainOperationVo<int>(DomainActionPerformed.None, 0);
            }

            bool entryAlreadyExists = existing.Entries.Any(x => x.Type == GiveawayEntryType.Daily && x.Date.ToLocalTime().Date == DateTime.Today.ToLocalTime().Date);

            if (entryAlreadyExists)
            {
                return new DomainOperationVo<int>(DomainActionPerformed.None, 0);
            }

            existing.Entries.Add(new GiveawayEntry
            {
                Date = DateTime.Now,
                Type = GiveawayEntryType.Daily,
                Points = 1
            });

            giveawayRepository.UpdateParticipant(giveawayId, existing);

            int countDailyEntries = existing.Entries.Where(x => x.Type == GiveawayEntryType.Daily).Sum(x => x.Points);

            return new DomainOperationVo<int>(DomainActionPerformed.Create, countDailyEntries);
        }

        public void ConfirmParticipant(Guid giveawayId, string referralCode)
        {
            GiveawayParticipant existing = giveawayRepository.GetParticipantByReferralCode(giveawayId, referralCode);

            if (existing != null && !existing.Entries.Any(x => x.Type == GiveawayEntryType.EmailConfirmed))
            {
                existing.Entries.Add(new GiveawayEntry
                {
                    Date = DateTime.Now,
                    Type = GiveawayEntryType.EmailConfirmed,
                    Points = 1
                });

                giveawayRepository.UpdateParticipant(giveawayId, existing);
            }
        }

        public void RemoveParticipant(Guid giveawayId, Guid participantId)
        {
            giveawayRepository.RemoveParticipant(giveawayId, participantId);
        }

        public void DeclareNotWinner(Guid giveawayId, Guid participantId)
        {
            GiveawayParticipant participant = giveawayRepository.GetParticipantById(giveawayId, participantId);

            if (participant != null)
            {
                participant.IsWinner = false;

                giveawayRepository.UpdateParticipant(giveawayId, participant);
            }
        }

        public void ClearParticipants(Guid giveawayId)
        {
            giveawayRepository.ClearParticipants(giveawayId);
        }

        public void PickSingleWinner(Guid giveawayId)
        {
            List<GiveawayParticipant> nonWinners = giveawayRepository.GetParticipants(giveawayId).Where(x => !x.IsWinner).ToList();

            if (nonWinners.Any())
            {
                Random rand = new Random(DateTime.Now.ToString().GetHashCode());

                int index = rand.Next(0, nonWinners.Count);

                GiveawayParticipant winner = nonWinners.ElementAt(index);

                winner.IsWinner = true;

                giveawayRepository.UpdateParticipant(giveawayId, winner);
            }
        }

        public void PickAllWinners(Guid giveawayId)
        {
            Task<GiveawayBasicInfoVo> task = Task.Run(async () => await giveawayRepository.GetBasicGiveawayById(giveawayId));

            task.Wait();

            GiveawayBasicInfoVo basicInfo = task.Result;

            List<GiveawayParticipant> allParticipants = giveawayRepository.GetParticipants(giveawayId).ToList();
            List<GiveawayParticipant> winners = allParticipants.Where(x => x.IsWinner).ToList();
            List<GiveawayParticipant> nonWinners = allParticipants.Where(x => !x.IsWinner).ToList();

            var allEntries = (from p in nonWinners
                              from e in p.Entries
                              select new
                              {
                                  Participant = p,
                                  Entry = e
                              }).ToList();

            int winnersToSelect = basicInfo.WinnerAmount - winners.Count;

            if (allParticipants.Count < winnersToSelect)
            {
                winnersToSelect = allParticipants.Count;
            }

            if (winnersToSelect > 0)
            {
                Random rand = new Random(DateTime.Now.ToString().GetHashCode());

                for (int i = 0; i < winnersToSelect; i++)
                {
                    int index = rand.Next(0, allEntries.Count);

                    GiveawayParticipant winner = allEntries.ElementAt(index).Participant;

                    winner.IsWinner = true;

                    giveawayRepository.UpdateParticipant(giveawayId, winner);

                    allEntries = allEntries.Except(allEntries.Where(x => x.Participant.Id == winner.Id)).ToList();
                }
            }

            giveawayRepository.UpdateGiveawayStatus(giveawayId, GiveawayStatus.Ended);
        }

        public void SetDates(IGiveawayBasicInfo model)
        {
            if (model != null)
            {
                if (model.StartDate == DateTime.MinValue)
                {
                    model.StartDate = DateTime.Now;
                }

                model.StartDate = model.StartDate.ToLocalTime();

                if (model.EndDate.HasValue)
                {
                    model.EndDate = model.EndDate.Value.ToLocalTime();
                }

                GiveawayStatus effectiveStatus = ComputeEffectiveStatus(model);

                if (effectiveStatus != GiveawayStatus.Draft)
                {
                    model.Status = effectiveStatus;
                }
            }
        }

        private static GiveawayStatus ComputeEffectiveStatus(IGiveawayBasicInfo model)
        {
            GiveawayStatus effectiveStatus = model.Status;

            if ((model.Status == GiveawayStatus.Draft || model.Status == GiveawayStatus.PendingStart) && model.StartDate <= DateTime.Now)
            {
                effectiveStatus = GiveawayStatus.OpenForEntries;
            }
            else if ((model.Status == GiveawayStatus.Draft || model.Status == GiveawayStatus.OpenForEntries) && model.StartDate >= DateTime.Now)
            {
                effectiveStatus = GiveawayStatus.PendingStart;
            }
            else if (model.Status != GiveawayStatus.Ended && model.EndDate.HasValue && DateTime.Now >= model.EndDate.Value)
            {
                effectiveStatus = GiveawayStatus.PickingWinners;
            }

            return effectiveStatus;
        }
    }
}