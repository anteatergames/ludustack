using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGamificationLevelAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<GamificationLevelViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, GamificationLevelViewModel viewModel);

        Task<OperationResultListVo<GamificationLevelViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultVo> GenerateNew(Guid currentUserId);

        Task<OperationResultVo> ValidateXp(Guid currentUserId, int xpToAchieve, Guid id);
    }
}