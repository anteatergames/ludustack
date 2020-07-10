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
                StartDate = DateTime.Now.AddHours(1)
            };

            return model;
        }

        public List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId)
        {
            List<GiveawayListItemVo> objs = repository.GetGiveawayListByUserId(userId);

            return objs;
        }

        public GiveawayBasicInfo GetGiveawayBasicInfoById(Guid id)
        {
            Task<GiveawayBasicInfo> task = Task.Run(async () => await repository.GetBasicGiveawayById(id));
            var model = task.Result;

            SetDates(model);

            return model;
        }

        public Giveaway GetGiveawayById(Guid id)
        {
            Task<Giveaway> task = Task.Run(async () => await repository.GetById(id));

            var model = task.Result;

            if (model != null)
            {
                SetDates(model);
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

        public DomainOperationVo<GiveawayParticipant> AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications, string referalCode, string referrer)
        {
            GiveawayParticipant participant;

            IQueryable<GiveawayParticipant> existing = repository.GetParticipants(giveawayId);
            bool exists = existing.Any(x => x.Email == email);

            if (exists)
            {
                participant = existing.First(x => x.Email == email);

                participant.GdprConsent = gdprConsent;
                participant.WantNotifications = wantNotifications;

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
                    var referrerParticipant = repository.GetParticipantByReferralCode(giveawayId, referrer);
                    if (referrerParticipant != null)
                    {
                        referrerParticipant.Entries.Add(new GiveawayEntry
                        {
                            Type = GiveawayEntryType.ReferralCode,
                            Points = 1
                        });

                        repository.UpdateParticipant(giveawayId, referrerParticipant);
                    }
                }

                return new DomainOperationVo<GiveawayParticipant>(DomainActionPerformed.Create, participant);
            }
        }

        public GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email)
        {
            GiveawayParticipant model = repository.GetParticipantByEmail(giveawayId, email);

            return model;
        }

        public void UpdateParticipantShortUrl(Guid giveawayId, string email, string shortUrl)
        {
            var existing = repository.GetParticipantByEmail(giveawayId, email);

            if (existing != null)
            {
                existing.ShortUrl = shortUrl;

                repository.UpdateParticipant(giveawayId, existing);
            }
        }

        public void ConfirmParticipant(Guid giveawayId, string referralCode)
        {
            var existing = repository.GetParticipantByReferralCode(giveawayId, referralCode);

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

        private static IGiveawayBasicInfo SetDates(IGiveawayBasicInfo model)
        {
            if (model.StartDate == DateTime.MinValue)
            {
                model.StartDate = DateTime.Now;
            }

            var timeZoneOffset = int.Parse(model.TimeZone ?? "0");

            model.StartDate = model.StartDate.AddHours(timeZoneOffset);

            if (model.EndDate.HasValue)
            {
                model.EndDate = model.EndDate.Value.AddHours(timeZoneOffset);
            }

            return model;
        }
    }
}