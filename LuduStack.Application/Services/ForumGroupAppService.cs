using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.ForumGroup;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ForumGroupAppService : BaseAppService, IForumGroupAppService
    {
        protected IForumGroupDomainService forumGroupDomainService;

        public ForumGroupAppService(IBaseAppServiceCommon baseAppServiceCommon, IForumGroupDomainService forumGroupDomainService) : base(baseAppServiceCommon)
        {
            this.forumGroupDomainService = forumGroupDomainService;
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountForumGroupQuery, int>(new CountForumGroupQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<IEnumerable<SelectListItemVo>> GetSelectList(Guid userId)
        {
            IEnumerable<ForumGroup> allGroups = await mediator.Query<GetForumGroupQuery, IEnumerable<ForumGroup>>(new GetForumGroupQuery());

            IEnumerable<SelectListItemVo> list = allGroups.OrderBy(x => x.Order).Select(x => new SelectListItemVo
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return list;
        }

        public async Task<OperationResultListVo<ForumGroupViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<ForumGroup> allModels = await mediator.Query<GetForumGroupQuery, IEnumerable<ForumGroup>>(new GetForumGroupQuery());

                IEnumerable<ForumGroupViewModel> vms = mapper.Map<IEnumerable<ForumGroup>, IEnumerable<ForumGroupViewModel>>(allModels);

                return new OperationResultListVo<ForumGroupViewModel>(vms.OrderBy(x => x.Order));
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ForumGroupViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumGroupViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                ForumGroup model = await mediator.Query<GetForumGroupByIdQuery, ForumGroup>(new GetForumGroupByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<ForumGroupViewModel>("Entity not found!");
                }

                ForumGroupViewModel vm = mapper.Map<ForumGroupViewModel>(model);

                return new OperationResultVo<ForumGroupViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumGroupViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteForumGroupCommand(currentUserId, id));

                return new OperationResultVo(true, "That Forum Group is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ForumGroupViewModel viewModel)
        {
            try
            {
                ForumGroup model;

                ForumGroup existing = await mediator.Query<GetForumGroupByIdQuery, ForumGroup>(new GetForumGroupByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<ForumGroup>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveForumGroupCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Forum Group saved!");
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
                ForumGroup model = await forumGroupDomainService.GenerateNew(currentUserId);

                ForumGroupViewModel newVm = mapper.Map<ForumGroupViewModel>(model);

                return new OperationResultVo<ForumGroupViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}