using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Services
{
    public class CrudBaseAppService<TEntity, TViewModel, TDomainService> : BaseAppService where TViewModel : BaseViewModel where TEntity : Entity where TDomainService : IDomainService<TEntity>
    {
        protected TDomainService domainService;

        public CrudBaseAppService(IBaseAppServiceCommon baseAppServiceCommon, TDomainService domainService) : base(baseAppServiceCommon)
        {
            this.domainService = domainService;
        }

        public virtual OperationResultVo<int> Count(Guid currentUserId)
        {
            try
            {
                int count = domainService.Count();

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public virtual OperationResultListVo<TViewModel> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<TEntity> allModels = domainService.GetAll();

                IEnumerable<TViewModel> vms = mapper.Map<IEnumerable<TEntity>, IEnumerable<TViewModel>>(allModels);

                return new OperationResultListVo<TViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TViewModel>(ex.Message);
            }
        }

        public virtual OperationResultVo GetAllIds(Guid currentUserId)
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

        public virtual OperationResultVo<TViewModel> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                TEntity model = domainService.GetById(id);

                if (model == null)
                {
                    return new OperationResultVo<TViewModel>("Entity not found!");
                }

                TViewModel vm = mapper.Map<TViewModel>(model);

                return new OperationResultVo<TViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<TViewModel>(ex.Message);
            }
        }

        public virtual OperationResultVo Remove(Guid currentUserId, Guid id)
        {
            try
            {
                domainService.Remove(id);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Entity is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual OperationResultVo<Guid> Save(Guid currentUserId, TViewModel viewModel)
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

                unitOfWork.Commit();

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
