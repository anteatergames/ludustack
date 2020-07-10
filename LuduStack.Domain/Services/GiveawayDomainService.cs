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

            SetDates(model);

            return model;
        }

        private static void SetRequiredProperties(Giveaway model)
        {
            if (model.Status == 0)
            {
                model.Status = GiveawayStatus.Draft;
            }
        }

        public DomainActionPerformed AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications)
        {
            GiveawayParticipant participant;

            IQueryable<GiveawayParticipant> existing = repository.GetParticipants(giveawayId);
            bool oneIsMine = existing.Any(x => x.Email == email);

            if (oneIsMine)
            {
                participant = existing.First(x => x.Email == email);

                participant.GdprConsent = gdprConsent;
                participant.WantNotifications = wantNotifications;

                repository.UpdateParticipant(giveawayId, participant);

                return DomainActionPerformed.Update;
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

                repository.AddParticipant(giveawayId, participant);

                return DomainActionPerformed.Create;
            }
        }

        public GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email)
        {
            GiveawayParticipant model = repository.GetParticipantByEmail(giveawayId, email);

            return model;
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