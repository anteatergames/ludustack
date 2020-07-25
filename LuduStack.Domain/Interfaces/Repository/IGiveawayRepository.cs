using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGiveawayRepository : IRepository<Giveaway>
    {
        Task<GiveawayBasicInfo> GetBasicGiveawayById(Guid id);

        List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId);

        IQueryable<GiveawayParticipant> GetParticipants(Guid giveawayId);

        void AddParticipant(Guid giveawayId, GiveawayParticipant participant);

        void UpdateParticipant(Guid giveawayId, GiveawayParticipant participant);

        void RemoveParticipant(Guid giveawayId, Guid participantId);

        GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email);

        Guid CheckParticipantByEmail(Guid giveawayId, string email);

        GiveawayParticipant GetParticipantByReferralCode(Guid giveawayId, string referrer);

        void ClearParticipants(Guid giveawayId);

        void UpdateGiveawayStatus(Guid giveawayId, GiveawayStatus newStatus);

        GiveawayParticipant GetParticipantById(Guid giveawayId, Guid participantId);
    }
}