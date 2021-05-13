using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayAppService
    {
        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetGiveawayForManagement(Guid currentUserId, Guid giveawayId);

        Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid giveawayId);

        Task<OperationResultVo> GetForDetails(Guid currentUserId, Guid giveawayId);

        Task<OperationResultVo> GetGiveawayParticipantInfo(Guid currentUserId, Guid giveawayId, string email);

        Task<OperationResultVo> GetGiveawaysByMe(Guid currentUserId);

        Task<OperationResultVo<Guid>> SaveGiveaway(Guid currentUserId, GiveawayViewModel viewModel);

        Task<OperationResultVo> DeleteGiveaway(Guid currentUserId, Guid giveawayId);

        Task<OperationResultVo> CheckParticipant(Guid currentUserId, Guid giveawayId, string sessionEmail);

        Task<OperationResultVo> EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm, string urlReferralBase);

        OperationResultVo DailyEntry(Guid currentUserId, Guid giveawayId, Guid participantId);

        OperationResultVo ConfirmParticipant(Guid currentUserId, Guid giveawayId, string referralCode);

        OperationResultVo RemoveParticipant(Guid currentUserId, Guid giveawayId, Guid participantId);

        OperationResultVo ClearParticipants(Guid currentUserId, Guid giveawayId);

        OperationResultVo PickSingleWinner(Guid currentUserId, Guid giveawayId);

        OperationResultVo PickAllWinners(Guid currentUserId, Guid giveawayId);

        OperationResultVo DeclareNotWinner(Guid currentUserId, Guid giveawayId, Guid participantId);

        Task<OperationResultVo> DuplicateGiveaway(Guid currentUserId, Guid giveawayId);
    }
}