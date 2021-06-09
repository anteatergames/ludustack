using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.ForumCategory;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ForumCategoryAppService : BaseAppService, IForumCategoryAppService
    {
        protected IForumCategoryDomainService gamificationLevelDomainService;

        public ForumCategoryAppService(IBaseAppServiceCommon baseAppServiceCommon, IForumCategoryDomainService gamificationLevelDomainService) : base(baseAppServiceCommon)
        {
            this.gamificationLevelDomainService = gamificationLevelDomainService;
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountForumCategoryQuery, int>(new CountForumCategoryQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ForumCategoryViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<ForumCategory> allModels = await mediator.Query<GetForumCategoryQuery, IEnumerable<ForumCategory>>(new GetForumCategoryQuery());

                IEnumerable<ForumCategoryViewModel> vms = mapper.Map<IEnumerable<ForumCategory>, IEnumerable<ForumCategoryViewModel>>(allModels);

                return new OperationResultListVo<ForumCategoryViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ForumCategoryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumCategoryViewModel>> GetForDetails(Guid currentUserId, Guid id)
        {
            try
            {
                ForumCategory model = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<ForumCategoryViewModel>("Entity not found!");
                }

                ForumCategoryViewModel vm = mapper.Map<ForumCategoryViewModel>(model);

                SetImagesToShow(vm);

                return new OperationResultVo<ForumCategoryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumCategoryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumCategoryViewModel>> GetForEdit(Guid currentUserId, Guid id)
        {
            try
            {
                ForumCategory model = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<ForumCategoryViewModel>("Entity not found!");
                }

                ForumCategoryViewModel vm = mapper.Map<ForumCategoryViewModel>(model);

                SetImagesToShow(vm);

                return new OperationResultVo<ForumCategoryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumCategoryViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteForumCategoryCommand(currentUserId, id));

                return new OperationResultVo(true, "That Forum Category is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ForumCategoryViewModel viewModel)
        {
            try
            {
                ForumCategory model;

                ForumCategory existing = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<ForumCategory>(viewModel);
                }

                if (!string.IsNullOrWhiteSpace(model.FeaturedImage) && model.FeaturedImage.Equals(Constants.DefaultFeaturedImage))
                {
                    model.FeaturedImage = null;
                }

                CommandResult result = await mediator.SendCommand(new SaveForumCategoryCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Forum Category saved!");
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
                ForumCategory model = await gamificationLevelDomainService.GenerateNew(currentUserId);

                ForumCategoryViewModel newVm = mapper.Map<ForumCategoryViewModel>(model);

                newVm.FeaturedImage = Constants.DefaultFeaturedImage;

                return new OperationResultVo<ForumCategoryViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void SetImagesToShow(ForumCategoryViewModel vm)
        {
            vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(vm.UserId, vm.FeaturedImage, ImageRenderType.Full);
        }
    }
}