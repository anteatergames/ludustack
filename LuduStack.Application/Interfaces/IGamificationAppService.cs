using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGamificationAppService
    {
        Task<OperationResultListVo<RankingViewModel>> GetAll();

        Task<OperationResultVo> FillProfileGamificationDetails(Guid currentUserId, ProfileViewModel vm);

        Task<OperationResultListVo<GamificationLevelViewModel>> GetAllLevels();

        Task<OperationResultListVo<UserBadgeViewModel>> GetBadgesByUserId(Guid userId);
    }
}