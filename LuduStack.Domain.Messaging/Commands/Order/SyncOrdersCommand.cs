using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SyncOrdersCommand : Command
    {
        public SyncOrdersCommand()
        {
        }

        public bool IsValid()
        {
            Result.Validation = new SyncOrdersCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SyncOrdersCommandHandler : CommandHandler, IRequestHandler<SyncOrdersCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IOrderRepository productRepository;
        protected readonly ISystemEventRepository systemEventRepository;
        protected readonly IExternalStore externalStore;

        public SyncOrdersCommandHandler(IUnitOfWork unitOfWork, IOrderRepository productRepository, ISystemEventRepository systemEventRepository, IExternalStore externalStore)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.systemEventRepository = systemEventRepository;
            this.externalStore = externalStore;
        }

        public async Task<CommandResult> Handle(SyncOrdersCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            List<Models.Order> ordersFromExternalStore = await this.externalStore.GetOrdersAsync();

            if (ordersFromExternalStore == null || !ordersFromExternalStore.Any())
            {
                return request.Result;
            }

            List<Models.Order> allLocalOrders = (await productRepository.GetAll()).ToList();

            foreach (Models.Order product in ordersFromExternalStore)
            {
                if (allLocalOrders.Any(x => x.Number.Equals(product.Number)))
                {
                    var existingOrder = allLocalOrders.First(x => x.Number.Equals(product.Number));
                    HandleExistingOrder(existingOrder, product);
                }
                else if (allLocalOrders.Any(x => x.StoreOrderNumber.ToLower().Equals(product.StoreOrderNumber.ToLower())))
                {
                    var existingOrder = allLocalOrders.First(x => x.StoreOrderNumber.ToLower().Equals(product.StoreOrderNumber.ToLower()));
                    HandleExistingOrder(existingOrder, product);
                }
                else
                {
                    await HandlNewOrder(product);
                }
            }

            await systemEventRepository.Add(new Models.SystemEvent { Type = SystemEventType.OrderSync });

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private async Task HandlNewOrder(Models.Order product)
        {
            await productRepository.Add(product);
        }

        private void HandleExistingOrder(Models.Order existingOrder, Models.Order incomingOrder)
        {
            existingOrder.Number = incomingOrder.Number;
            existingOrder.StoreOrderNumber = incomingOrder.StoreOrderNumber;
            existingOrder.TotalProductsValue = incomingOrder.TotalProductsValue;
            existingOrder.FreightValue = incomingOrder.FreightValue;
            existingOrder.TotalOrderValue = incomingOrder.TotalOrderValue;

            existingOrder.CreateDate = incomingOrder.CreateDate;

            existingOrder.Situation = incomingOrder.Situation;

            existingOrder.Items = incomingOrder.Items;

            productRepository.Update(existingOrder);
        }
    }
}