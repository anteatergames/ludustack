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

        GiveawayBasicInfo GetGiveawayBasicInfoById(Guid id);

        Giveaway GetGiveawayById(Guid id);

        List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId);

        DomainActionPerformed AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications);

        GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email);
    }
}