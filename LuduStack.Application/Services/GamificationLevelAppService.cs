using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.GamificationLevel;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GamificationLevelAppService : CrudBaseAppService<GamificationLevel, GamificationLevelViewModel, IGamificationLevelDomainService>, IGamificationLevelAppService
    {
        public GamificationLevelAppService(IBaseAppServiceCommon baseAppServiceCommon, IGamificationLevelDomainService gamificationLevelDomainService) : base(baseAppServiceCommon, gamificationLevelDomainService)
        {
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountGamificationLevelQuery, int>(new CountGamificationLevelQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GenerateNew(Guid currentUserId)
        {
            try
            {
                GamificationLevel model = await domainService.GenerateNew(currentUserId);

                GamificationLevelViewModel newVm = mapper.Map<GamificationLevelViewModel>(model);

                return new OperationResultVo<GamificationLevelViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> ValidateXp(Guid currentUserId, int xpToAchieve, Guid id)
        {
            try
            {
                bool valid = await domainService.ValidateXp(xpToAchieve, id);

                return new OperationResultVo(valid);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}