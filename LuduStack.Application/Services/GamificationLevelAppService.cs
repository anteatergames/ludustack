using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.GamificationLevel;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GamificationLevelAppService : BaseAppService, IGamificationLevelAppService
    {
        protected IGamificationLevelDomainService gamificationLevelDomainService;

        public GamificationLevelAppService(IBaseAppServiceCommon baseAppServiceCommon, IGamificationLevelDomainService gamificationLevelDomainService) : base(baseAppServiceCommon)
        {
            this.gamificationLevelDomainService = gamificationLevelDomainService;
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

        public async Task<OperationResultListVo<GamificationLevelViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<GamificationLevel> allModels = await mediator.Query<GetGamificationLevelQuery, IEnumerable<GamificationLevel>>(new GetGamificationLevelQuery());

                IEnumerable<GamificationLevelViewModel> vms = mapper.Map<IEnumerable<GamificationLevel>, IEnumerable<GamificationLevelViewModel>>(allModels);

                return new OperationResultListVo<GamificationLevelViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<GamificationLevelViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GamificationLevelViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                GamificationLevel model = await mediator.Query<GetGamificationLevelByIdQuery, GamificationLevel>(new GetGamificationLevelByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<GamificationLevelViewModel>("Entity not found!");
                }

                GamificationLevelViewModel vm = mapper.Map<GamificationLevelViewModel>(model);

                return new OperationResultVo<GamificationLevelViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GamificationLevelViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteGamificationLevelCommand(currentUserId, id));

                return new OperationResultVo(true, "That Gamification Level is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, GamificationLevelViewModel viewModel)
        {
            try
            {
                GamificationLevel model;

                GamificationLevel existing = await mediator.Query<GetGamificationLevelByIdQuery, GamificationLevel>(new GetGamificationLevelByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<GamificationLevel>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveGamificationLevelCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Gamification Level saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GenerateNew(Guid currentUserId)
        {
            try
            {
                GamificationLevel model = await gamificationLevelDomainService.GenerateNew(currentUserId);

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
                bool valid = await gamificationLevelDomainService.ValidateXp(xpToAchieve, id);

                return new OperationResultVo(valid);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}