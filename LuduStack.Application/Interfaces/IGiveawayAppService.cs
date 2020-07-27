using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayAppService
    {
        OperationResultVo GenerateNew(Guid currentUserId);

        OperationResultVo GetGiveawayForManagement(Guid currentUserId, Guid id);

        OperationResultVo GetForEdit(Guid currentUserId, Guid giveawayId);

        OperationResultVo GetForDetails(Guid currentUserId, Guid id);

        OperationResultVo GetGiveawayParticipantInfo(Guid currentUserId, Guid giveawayId, string email);

        OperationResultVo GetGiveawaysByMe(Guid currentUserId);

        OperationResultVo<Guid> SaveGiveaway(Guid currentUserId, GiveawayViewModel vm);

        OperationResultVo DeleteGiveaway(Guid currentUserId, Guid giveawayId);

        OperationResultVo CheckParticipant(Guid currentUserId, Guid giveawayId, string sessionEmail);

        OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase);

        OperationResultVo DailyEntry(Guid currentUserId, Guid giveawayId, Guid participantId);

        OperationResultVo ConfirmParticipant(Guid currentUserId, Guid giveawayId, string referralCode);

        OperationResultVo RemoveParticipant(Guid currentUserId, Guid giveawayId, Guid participantId);

        OperationResultVo ClearParticipants(Guid currentUserId, Guid giveawayId);

        OperationResultVo PickSingleWinner(Guid currentUserId, Guid giveawayId);

        OperationResultVo PickAllWinners(Guid currentUserId, Guid giveawayId);

        OperationResultVo DeclareNotWinner(Guid currentUserId, Guid giveawayId, Guid participantId);

        OperationResultVo DuplicateGiveaway(Guid currentUserId, Guid giveawayId);
    }
}