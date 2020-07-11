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
        List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId);
        DomainOperationVo<GiveawayParticipant> AddParticipant(Guid giveawayId, string email, bool gdprConsent, bool wantNotifications, string referralCode, string referrer);
        GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email);
        void UpdateParticipantShortUrl(Guid giveawayId, string email, string shortUrl);
        void ConfirmParticipant(Guid giveawayId, string referralCode);
        void RemoveParticipant(Guid giveawayId, Guid participantId);
        void ClearParticipants(Guid giveawayId);
        void PickSingleWinner(Guid giveawayId);
        void PickAllWinners(Guid giveawayId);
    }
}