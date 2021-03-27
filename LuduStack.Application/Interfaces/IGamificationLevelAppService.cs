using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGamificationLevelAppService : ICrudAppService<GamificationLevelViewModel>
    {
        Task<OperationResultListVo<GamificationLevelViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultVo> GenerateNew(Guid currentUserId);

        Task<OperationResultVo> ValidateXp(Guid currentUserId, int xpToAchieve, Guid id);
    }
}