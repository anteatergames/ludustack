using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Store;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Store;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ProductAppService : BaseAppService, IProductAppService
    {
        public ProductAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountProductQuery, int>(new CountProductQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ProductListViewModel>> Get(Guid currentUserId)
        {
            ProductListViewModel model = new ProductListViewModel();

            try
            {
                IEnumerable<Product> allModels = await mediator.Query<GetProductQuery, IEnumerable<Product>>(new GetProductQuery());

                IEnumerable<ProductViewModel> vms = mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(allModels);

                model.Elements = vms.ToList();

                foreach (ProductViewModel element in model.Elements)
                {
                    FormatToShow(element);
                }

                return new OperationResultVo<ProductListViewModel>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ProductListViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ProductViewModel>> GetByOwner(Guid currentUserId, Guid userId)
        {
            try
            {
                IEnumerable<Product> products = await mediator.Query<GetProductByOwnerQuery, IEnumerable<Product>>(new GetProductByOwnerQuery(userId));

                if (products == null)
                {
                    return new OperationResultListVo<ProductViewModel>("Entity not found!");
                }

                IEnumerable<ProductViewModel> vms = mapper.Map<IEnumerable<ProductViewModel>>(products);

                foreach (ProductViewModel element in vms)
                {
                    FormatToShow(element);
                }

                return new OperationResultListVo<ProductViewModel>(vms.ToList());
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ProductViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ProductViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                Product model = await mediator.Query<GetProductByIdQuery, Product>(new GetProductByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<ProductViewModel>("Entity not found!");
                }

                ProductViewModel vm = mapper.Map<ProductViewModel>(model);

                FormatToShow(vm);

                await SetOwners(vm);

                return new OperationResultVo<ProductViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ProductViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteProductCommand(currentUserId, id));

                return new OperationResultVo(true, "That Product is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ProductViewModel viewModel)
        {
            try
            {
                Product model;

                Product existing = await mediator.Query<GetProductByIdQuery, Product>(new GetProductByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Product>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveProductCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Product saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Sync(Guid currentUserId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new SyncProductsCommand());

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo(false, message);
                }

                return new OperationResultVo(true, "Products synched!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private async Task SetOwners(ProductViewModel vm)
        {
            IEnumerable<Guid> ownersIds = vm.Owners.Select(x => x.UserId);

            IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(ownersIds));

            vm.OwnersProfiles = new List<ProfileViewModel>();
            foreach (UserProfileEssentialVo profileEssential in profiles)
            {
                StorePartnerViewModel owner = vm.Owners.First(x => x.UserId == profileEssential.UserId);

                ProfileViewModel profile = new ProfileViewModel
                {
                    UserId = profileEssential.UserId,
                    Handler = profileEssential.Handler,
                    Location = profileEssential.Location,
                    CreateDate = profileEssential.CreateDate,
                    LastUpdateDate = profileEssential.LastUpdateDate,
                    Name = profileEssential.Name,
                    ProfileImageUrl = UrlFormatter.ProfileImage(profileEssential.UserId, Constants.HugeAvatarSize),
                    CoverImageUrl = UrlFormatter.ProfileCoverImage(profileEssential.UserId, profileEssential.Id, profileEssential.LastUpdateDate, profileEssential.HasCoverImage, Constants.ProfileCoverSize)
                };

                vm.OwnersProfiles.Add(profile);
            }

            vm.OwnersProfiles = vm.OwnersProfiles.OrderByDescending(x => x.LastUpdateDate).ToList();
        }

        private void FormatToShow(ProductViewModel element)
        {
            element.StoreHandler = string.IsNullOrWhiteSpace(element.StoreHandler) ? element.Name.Slugify() : element.StoreHandler;
            element.StoreUrl = UrlFormatter.StoreProduct(element.StoreHandler);
        }
    }
}