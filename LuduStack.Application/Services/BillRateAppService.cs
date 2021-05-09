using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.BillRate;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class BillRateAppService : ProfileBaseAppService, IBillRateAppService
    {
        public BillRateAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }

        public Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultVo<int>("Not Implemented"));
        }

        public Task<OperationResultListVo<BillRateViewModel>> GetAll(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultListVo<BillRateViewModel>("Not Implemented"));
        }

        public Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultListVo<Guid>("Not Implemented"));
        }

        public Task<OperationResultVo<BillRateViewModel>> GetById(Guid currentUserId, Guid id)
        {
            return Task.FromResult(new OperationResultVo<BillRateViewModel>("Not Implemented"));
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new DeleteBillRateCommand(currentUserId, id));

                if (!result.Validation.IsValid)
                {
                    return new OperationResultVo(result.Validation.Errors.First().ErrorMessage);
                }
                else
                {
                    return new OperationResultVo(true, "That Bill Rate is gone now!");
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, BillRateViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                BillRate model = mapper.Map<BillRate>(viewModel);
                string msg = "Bill Rate Saved";

                CommandResult<Guid> result = await mediator.SendCommand<SaveBillRateCommand, Guid>(new SaveBillRateCommand(currentUserId, viewModel.BillRateType, viewModel.ArtStyle.Value, viewModel.SoundStyle, viewModel.GameElement, viewModel.HourPrice, viewModel.HourQuantity));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;
                model.Id = result.Result;

                return new OperationResultVo<Guid>(model.Id, pointsEarned, msg);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public OperationResultVo GenerateNew(Guid currentUserId)
        {
            try
            {
                BillRateViewModel newVm = new BillRateViewModel
                {
                    UserId = currentUserId,
                    GameElement = GameElement.ConceptArt,
                    HourPrice = 30,
                    HourQuantity = 8
                };

                return new OperationResultVo<BillRateViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetByMe(Guid currentUserId)
        {
            try
            {
                IEnumerable<BillRate> rates = await mediator.Query<GetBillRatesByUserIdQuery, IEnumerable<BillRate>>(new GetBillRatesByUserIdQuery(currentUserId));

                IEnumerable<BillRateViewModel> voList = rates.Select(x => new BillRateViewModel
                {
                    Id = x.Id,
                    BillRateType = x.BillRateType,
                    GameElement = x.GameElement,
                    ArtStyle = x.ArtStyle,
                    SoundStyle = x.SoundStyle,
                    HourPrice = x.HourPrice,
                    HourQuantity = x.HourQuantity,
                    ElementPrice = x.HourPrice * x.HourQuantity
                });

                IOrderedEnumerable<BillRateViewModel> orderedList = voList.OrderBy(x => x.GameElement).ThenBy(x => x.ArtStyle).ThenBy(x => x.SoundStyle);

                return new OperationResultListVo<BillRateViewModel>(orderedList);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid id)
        {
            try
            {
                BillRate existing = await mediator.Query<GetBillRateByIdQuery, BillRate>(new GetBillRateByIdQuery(id));

                BillRateViewModel vm = mapper.Map<BillRateViewModel>(existing);

                SetPermissions(currentUserId, vm);

                return new OperationResultVo<BillRateViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void SetPermissions(Guid currentUserId, BillRateViewModel vm)
        {
            SetBasePermissions(currentUserId, vm);
        }
    }
}