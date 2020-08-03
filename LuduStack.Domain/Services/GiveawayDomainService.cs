using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
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
    public class GiveawayDomainService : BaseDomainMongoService<Giveaway, IGiveawayRepository>, IGiveawayDomainService
    {
        public GiveawayDomainService(IGiveawayRepository repository) : base(repository)
        {
        }

        public override Guid Add(Giveaway model)
        {
            SetRequiredProperties(model);

            return base.Add(model);
        }

        public override Guid Update(Giveaway model)
        {
            SetRequiredProperties(model);

            return base.Update(model);
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

        public List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId)
        {
            List<GiveawayListItemVo> objs = repository.GetGiveawayListByUserId(userId);

            foreach (GiveawayListItemVo item in objs)
            {
                SetDates(item);
            }

            return objs;
        }

        public override Giveaway GetById(Guid id)
        {
            Task<Giveaway> task = Task.Run(async () => await repository.GetById(id));
            Giveaway model = task.Result;

            SetDates(model);

            return model;
        }

        public GiveawayBasicInfo GetGiveawayBasicInfoById(Guid id)
        {
            Task<GiveawayBasicInfo> task = Task.Run(async () => await repository.GetBasicGiveawayById(id));
            GiveawayBasicInfo model = task.Result;

            if (model.Status == GiveawayStatus.Ended)
            {
                model.Winners = repository.GetParticipants(id).Where(x => x.IsWinner).ToList();
            }

            SetDates(model);

            return model;
        }

        public Giveaway Duplicate(Guid giveawayId)
        {
            Task<Giveaway> task = Task.Run(async () => await repository.GetById(giveawayId));
            Giveaway model = task.Result;

            var copy = model.Copy();

            copy.Id = Guid.Empty;

            copy.Name = string.Format("{0} (Copy {1})", copy.Name, DateTime.Now.ToString("yyyyMMddhhmmss"));

            copy.Status = GiveawayStatus.Draft;

            copy.StartDate = DateTime.Today.AddDays(1);
            copy.EndDate = copy.StartDate.AddDays(1);

            copy.Participants = new List<GiveawayParticipant>();

            repository.Add(copy);

            return copy;
        }

        public DomainOperationVo<GiveawayParticipant> AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications, string referalCode, string referrer, GiveawayEntryType? entryType)
        {
            GiveawayParticipant participant;

            IQueryable<GiveawayParticipant> existing = repository.GetParticipants(giveawayId);
            bool exists = existing.Any(x => x.Email == email);

            if (exists)
            {
                participant = existing.First(x => x.Email == email);

                repository.UpdateParticipant(giveawayId, participant);

                return new DomainOperationVo<GiveawayParticipant>(DomainActionPerformed.Update, participant);
            }
            else
            {
                participant = new GiveawayParticipant
                {
                    Email = email,
                    GdprConsent = gdprConsent,
                    WantNotifications = wantNotifications
                };

                participant.Entries.Add(new GiveawayEntry
                {
                    Type = GiveawayEntryType.LoginOrEmail,
                    Points = 1
                });

                participant.ReferralCode = referalCode;

                repository.AddParticipant(giveawayId, participant);

                if (!string.IsNullOrWhiteSpace(referrer))
                {
                    GiveawayParticipant referrerParticipant = repository.GetParticipantByReferralCode(giveawayId, referrer);
                    if (referrerParticipant != null)
                    {
                        referrerParticipant.Entries.Add(new GiveawayEntry
                        {
                            Type = entryType ?? GiveawayEntryType.ReferralCode,
                            Points = 1
                        });

                        repository.UpdateParticipant(giveawayId, referrerParticipant);
                    }
                }

                return new DomainOperationVo<GiveawayParticipant>(DomainActionPerformed.Create, participant);
            }
        }

        public DomainOperationVo<int> DailyEntry(Guid giveawayId, Guid participantId)
        {
            var existing = repository.GetParticipantById(giveawayId, participantId);
            if (existing == null)
            {
                return new DomainOperationVo<int>(DomainActionPerformed.None, 0);
            }

            var entryAlreadyExists = existing.Entries.Any(x => x.Type == GiveawayEntryType.Daily && x.Date.ToLocalTime().Date == DateTime.Today.ToLocalTime().Date);

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

            repository.UpdateParticipant(giveawayId, existing);

            var countDailyEntries = existing.Entries.Where(x => x.Type == GiveawayEntryType.Daily).Sum(x => x.Points);

            return new DomainOperationVo<int>(DomainActionPerformed.Create, countDailyEntries);
        }

        public GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email)
        {
            GiveawayParticipant model = repository.GetParticipantByEmail(giveawayId, email);

            return model;
        }

        public bool CheckParticipantByEmail(Guid giveawayId, string email)
        {
            var guid = repository.CheckParticipantByEmail(giveawayId, email);

            return guid != default(Guid);
        }

        public void UpdateParticipantShortUrl(Guid giveawayId, string email, string shortUrl)
        {
            GiveawayParticipant existing = repository.GetParticipantByEmail(giveawayId, email);

            if (existing != null)
            {
                existing.ShortUrl = shortUrl;

                repository.UpdateParticipant(giveawayId, existing);
            }
        }

        public void ConfirmParticipant(Guid giveawayId, string referralCode)
        {
            GiveawayParticipant existing = repository.GetParticipantByReferralCode(giveawayId, referralCode);

            if (existing != null && !existing.Entries.Any(x => x.Type == GiveawayEntryType.EmailConfirmed))
            {
                existing.Entries.Add(new GiveawayEntry
                {
                    Type = GiveawayEntryType.EmailConfirmed,
                    Points = 1
                });

                repository.UpdateParticipant(giveawayId, existing);
            }
        }

        public void RemoveParticipant(Guid giveawayId, Guid participantId)
        {
            repository.RemoveParticipant(giveawayId, participantId);
        }

        public void DeclareNotWinner(Guid giveawayId, Guid participantId)
        {
            GiveawayParticipant participant = repository.GetParticipantById(giveawayId, participantId);

            if (participant != null)
            {
                participant.IsWinner = false;

                repository.UpdateParticipant(giveawayId, participant);
            }
        }

        public void ClearParticipants(Guid giveawayId)
        {
            repository.ClearParticipants(giveawayId);
        }

        public void PickSingleWinner(Guid giveawayId)
        {
            List<GiveawayParticipant> nonWinners = repository.GetParticipants(giveawayId).Where(x => !x.IsWinner).ToList();

            if (nonWinners.Any())
            {
                Random rand = new Random(DateTime.Now.ToString().GetHashCode());

                int index = rand.Next(0, nonWinners.Count);

                GiveawayParticipant winner = nonWinners.ElementAt(index);

                winner.IsWinner = true;

                repository.UpdateParticipant(giveawayId, winner);
            }
        }

        public void PickAllWinners(Guid giveawayId)
        {
            Task<GiveawayBasicInfo> task = Task.Run(async () => await repository.GetBasicGiveawayById(giveawayId));

            task.Wait();

            GiveawayBasicInfo basicInfo = task.Result;

            List<GiveawayParticipant> allParticipants = repository.GetParticipants(giveawayId).ToList();
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

                    repository.UpdateParticipant(giveawayId, winner);

                    allEntries = allEntries.Except(allEntries.Where(x => x.Participant.Id == winner.Id)).ToList();
                }
            }

            repository.UpdateGiveawayStatus(giveawayId, GiveawayStatus.Ended);
        }

        private static IGiveawayBasicInfo SetDates(IGiveawayBasicInfo model)
        {
            if (model != null)
            {
                if (model.StartDate == DateTime.MinValue)
                {
                    model.StartDate = DateTime.Now;
                }

                //int timeZoneOffset = int.Parse(model.TimeZone ?? "0");

                model.StartDate = model.StartDate.ToLocalTime();//.AddHours(timeZoneOffset);

                if (model.EndDate.HasValue)
                {
                    model.EndDate = model.EndDate.Value.ToLocalTime();//.AddHours(timeZoneOffset);
                }

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

                if (effectiveStatus != GiveawayStatus.Draft)
                {
                    model.Status = effectiveStatus;
                }
            }

            return model;
        }

        private static void SetRequiredProperties(Giveaway model)
        {
            if (model.Status == 0)
            {
                model.Status = GiveawayStatus.Draft;
            }
        }
    }
}