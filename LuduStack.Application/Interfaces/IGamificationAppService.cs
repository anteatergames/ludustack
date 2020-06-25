using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IGamificationAppService
    {
        OperationResultListVo<RankingViewModel> GetAll();

        OperationResultVo FillProfileGamificationDetails(Guid currentUserId, ref ProfileViewModel vm);

        OperationResultListVo<GamificationLevelViewModel> GetAllLevels();

        OperationResultListVo<UserBadgeViewModel> GetBadgesByUserId(Guid userId);
    }
}