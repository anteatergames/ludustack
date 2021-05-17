using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGiveawayDomainService
    {
        Giveaway GenerateNewGiveaway(Guid userId);

        void SetDates(IGiveawayBasicInfo model);

        DomainOperationVo<int> DailyEntry(Guid giveawayId, Guid participantId);

        void ConfirmParticipant(Guid giveawayId, string referralCode);

        void RemoveParticipant(Guid giveawayId, Guid participantId);

        void ClearParticipants(Guid giveawayId);

        void PickSingleWinner(Guid giveawayId);

        void PickAllWinners(Guid giveawayId);

        void DeclareNotWinner(Guid giveawayId, Guid participantId);
    }
}