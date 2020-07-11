using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayAppService
    {
        OperationResultVo GenerateNew(Guid currentUserId);
        OperationResultVo GetGiveawayFullById(Guid currentUserId, Guid giveawayId);
        OperationResultVo GetGiveawayBasicInfoById(Guid currentUserId, Guid giveawayId);
        OperationResultVo GetGiveawayParticipantInfo(Guid currentUserId, Guid giveawayId, string email);
        OperationResultVo GetGiveawaysByMe(Guid currentUserId);
        OperationResultVo<Guid> SaveGiveaway(Guid currentUserId, GiveawayViewModel vm);
        OperationResultVo RemoveGiveaway(Guid currentUserId, Guid giveawayId);
        OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase);
        OperationResultVo ConfirmParticipant(Guid currentUserId, Guid giveawayId, string referralCode);
        OperationResultVo RemoveParticipant(Guid currentUserId, Guid giveawayId, Guid participantId);
        OperationResultVo ClearParticipants(Guid currentUserId, Guid giveawayId);
        OperationResultVo PickSingleWinner(Guid currentUserId, Guid giveawayId);
        OperationResultVo PickAllWinners(Guid currentUserId, Guid giveawayId);
    }
}