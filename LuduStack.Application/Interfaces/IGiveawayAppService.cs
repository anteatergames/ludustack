using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayAppService
    {
        OperationResultVo GenerateNew(Guid currentUserId);
        OperationResultVo GetGiveawayById(Guid currentUserId, Guid id);
        OperationResultVo GetGiveawaysByMe(Guid currentUserId);
        OperationResultVo<Guid> SaveGiveaway(Guid currentUserId, GiveawayViewModel vm);
        OperationResultVo RemoveGiveaway(Guid currentUserId, Guid id);
        OperationResultVo EnterGiveaway(Guid currentUserId, GiveawayEnterViewModel vm);
    }
}