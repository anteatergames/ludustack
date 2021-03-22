using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class CrudBaseAppService<TEntity, TViewModel, TDomainService> : BaseAppService where TViewModel : BaseViewModel where TEntity : Entity where TDomainService : IDomainService<TEntity>
    {
        protected TDomainService domainService;

        public CrudBaseAppService(IBaseAppServiceCommon baseAppServiceCommon, TDomainService domainService) : base(baseAppServiceCommon)
        {
            this.domainService = domainService;
        }

        public virtual Task<OperationResultListVo<TViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<TEntity> allModels = domainService.GetAll();

                IEnumerable<TViewModel> vms = mapper.Map<IEnumerable<TEntity>, IEnumerable<TViewModel>>(allModels);

                return Task.FromResult(new OperationResultListVo<TViewModel>(vms));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OperationResultListVo<TViewModel>(ex.Message));
            }
        }

        public virtual async Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = domainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual Task<OperationResultVo<TViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                TEntity model = domainService.GetById(id);

                if (model == null)
                {
                    return Task.FromResult(new OperationResultVo<TViewModel>("Entity not found!"));
                }

                TViewModel vm = mapper.Map<TViewModel>(model);

                return Task.FromResult(new OperationResultVo<TViewModel>(vm));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OperationResultVo<TViewModel>(ex.Message));
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                domainService.Remove(id);

                await unitOfWork.Commit();

                return new OperationResultVo(true, "That Entity is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, TViewModel viewModel)
        {
            try
            {
                TEntity model;

                TEntity existing = domainService.GetById(viewModel.Id);
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<TEntity>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    domainService.Add(model);
                    viewModel.Id = model.Id;
                }
                else
                {
                    domainService.Update(model);
                }

                await unitOfWork.Commit();

                viewModel.Id = model.Id;

                return new OperationResultVo<Guid>(model.Id);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }
    }
}