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
        List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId);
        IQueryable<GiveawayParticipant> GetParticipants(Guid giveawayId);
        void AddParticipant(Guid giveawayId, GiveawayParticipant participant);
        void UpdateParticipant(Guid giveawayId, GiveawayParticipant participant);
        void RemoveParticipant(Guid giveawayId, Guid participantId);
    }
}