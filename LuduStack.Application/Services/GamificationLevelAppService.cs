using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GamificationLevelAppService : CrudBaseAppService<GamificationLevel, GamificationLevelViewModel, IGamificationLevelDomainService>, IGamificationLevelAppService
    {
        public GamificationLevelAppService(IBaseAppServiceCommon baseAppServiceCommon, IGamificationLevelDomainService gamificationLevelDomainService) : base(baseAppServiceCommon, gamificationLevelDomainService)
        {
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
