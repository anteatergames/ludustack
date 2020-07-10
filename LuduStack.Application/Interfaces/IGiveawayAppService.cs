using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayAppService
    {
        OperationResultVo GenerateNew(Guid currentUserId);
        OperationResultVo GetGiveawayBasicInfoById(Guid currentUserId, Guid giveawayId);
        OperationResultVo GetGiveawayParticipantInfo(Guid currentUserId, Guid giveawayId, string email);
        OperationResultVo GetGiveawaysByMe(Guid currentUserId);
        OperationResultVo<Guid> SaveGiveaway(Guid currentUserId, GiveawayViewModel vm);
        OperationResultVo RemoveGiveaway(Guid currentUserId, Guid giveawayId);
        OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase);
        OperationResultVo ConfirmParticipant(Guid currentUserId, Guid giveawayId, string referralCode);
    }
}