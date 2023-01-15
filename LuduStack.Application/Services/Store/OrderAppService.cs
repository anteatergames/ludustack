using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Store;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class OrderAppService : BaseAppService, IOrderAppService
    {
        public OrderAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountOrderQuery, int>(new CountOrderQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultVo<OrderListViewModel>> Get(Guid currentUserId)
        {
            OrderListViewModel model = new OrderListViewModel();

            try
            {
                IEnumerable<Order> allModels = await mediator.Query<GetOrderQuery, IEnumerable<Order>>(new GetOrderQuery());

                IEnumerable<OrderViewModel> vms = mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(allModels);

                model.Elements = vms.ToList();

                foreach (OrderViewModel element in model.Elements)
                {
                    FormatToShow(element);
                }

                return new OperationResultVo<OrderListViewModel>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<OrderListViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<OrderViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                Order model = await mediator.Query<GetOrderByIdQuery, Order>(new GetOrderByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<OrderViewModel>("Entity not found!");
                }

                OrderViewModel vm = mapper.Map<OrderViewModel>(model);

                FormatToShow(vm);

                return new OperationResultVo<OrderViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<OrderViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteOrderCommand(currentUserId, id));

                return new OperationResultVo(true, "That Order is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, OrderViewModel viewModel)
        {
            try
            {
                Order model;

                Order existing = await mediator.Query<GetOrderByIdQuery, Order>(new GetOrderByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Order>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveOrderCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Order saved!");
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
                CommandResult result = await mediator.SendCommand(new SyncOrdersCommand());

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo(false, message);
                }

                return new OperationResultVo(true, "Orders synched!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void FormatToShow(OrderViewModel element)
        {
            // TODO placeholder
        }
    }
}