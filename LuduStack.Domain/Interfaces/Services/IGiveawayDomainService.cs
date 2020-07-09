using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGiveawayDomainService : IDomainService<Giveaway>
    {
        Giveaway GenerateNewGiveaway(Guid userId);

        Giveaway GetGiveawayById(Guid id);

        List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId);

        DomainActionPerformed AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications);
    }
}