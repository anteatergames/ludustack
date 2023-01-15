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
    public class StorePartnershipAppService : BaseAppService, IStorePartnershipAppService
    {
        public StorePartnershipAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountStorePartnershipQuery, int>(new CountStorePartnershipQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<StorePartnershipViewModel>> Get(Guid currentUserId)
        {
            StorePartnershipViewModel model = new StorePartnershipViewModel();

            try
            {
                IEnumerable<StorePartnership> allModels = await mediator.Query<GetStorePartnershipQuery, IEnumerable<StorePartnership>>(new GetStorePartnershipQuery());

                IEnumerable<StorePartnershipViewModel> vms = mapper.Map<IEnumerable<StorePartnership>, IEnumerable<StorePartnershipViewModel>>(allModels);

                List<StorePartnershipViewModel> list = vms.ToList();

                foreach (StorePartnershipViewModel vm in list)
                {
                    FormatToShow(vm);
                }

                return new OperationResultListVo<StorePartnershipViewModel>(list);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<StorePartnershipViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<StorePartnershipViewModel>> GetByPartner(Guid currentUserId, Guid userId)
        {
            StorePartnershipViewModel vm;

            try
            {
                StorePartnership storePartnership = await mediator.Query<GetStorePartnershipByPartnerQuery, StorePartnership>(new GetStorePartnershipByPartnerQuery(userId));

                if (storePartnership == null)
                {
                    vm = new StorePartnershipViewModel
                    {
                        PartnerUserId = userId
                    };
                }
                else
                {
                    vm = mapper.Map<StorePartnershipViewModel>(storePartnership);
                }

                FormatToShow(vm);

                return new OperationResultVo<StorePartnershipViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<StorePartnershipViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<StorePartnershipViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                StorePartnership model = await mediator.Query<GetStorePartnershipByIdQuery, StorePartnership>(new GetStorePartnershipByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<StorePartnershipViewModel>("Entity not found!");
                }

                StorePartnershipViewModel vm = mapper.Map<StorePartnershipViewModel>(model);

                FormatToShow(vm);

                await SetPartnerInformation(vm);

                return new OperationResultVo<StorePartnershipViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<StorePartnershipViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteStorePartnershipCommand(currentUserId, id));

                return new OperationResultVo(true, "That StorePartnership is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, StorePartnershipViewModel viewModel)
        {
            try
            {
                StorePartnership model;

                StorePartnership existing = await mediator.Query<GetStorePartnershipByIdQuery, StorePartnership>(new GetStorePartnershipByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<StorePartnership>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveStorePartnershipCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "StorePartnership saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Sync(Guid currentUserId, Guid partnerUserId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new SyncStorePartnershipsCommand(partnerUserId));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo(false, message);
                }

                return new OperationResultVo(true, "StorePartnerships synched!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private async Task SetPartnerInformation(StorePartnershipViewModel vm)
        {
            UserProfileEssentialVo profileEssential = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(vm.PartnerUserId));

            vm.PartnerProfile = new ProfileViewModel();
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

            vm.PartnerProfile = profile;
        }

        private void FormatToShow(StorePartnershipViewModel element)
        {
            foreach (StorePartnershipTransactionViewModel transaction in element.Transactions)
            {
                transaction.TypeText = transaction.Type.ToUiInfo().Display;
            }
        }
    }
}