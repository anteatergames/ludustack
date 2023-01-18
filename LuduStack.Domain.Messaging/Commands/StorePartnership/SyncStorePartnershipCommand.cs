using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SyncStorePartnershipsCommand : Command
    {
        public Guid PartnerUserId { get; private set; }

        public SyncStorePartnershipsCommand(Guid partnerUserId)
        {
            PartnerUserId = partnerUserId;
        }

        public bool IsValid()
        {
            Result.Validation = new SyncStorePartnershipsCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SyncStorePartnershipsCommandHandler : CommandHandler, IRequestHandler<SyncStorePartnershipsCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ISystemEventRepository systemEventRepository;
        protected readonly IStorePartnershipRepository storePartnershipRepository;
        protected readonly IProductRepository productRepository;
        protected readonly IOrderRepository orderRepository;
        int partnerPercentage = 10;

        public SyncStorePartnershipsCommandHandler(IUnitOfWork unitOfWork, ISystemEventRepository systemEventRepository, IStorePartnershipRepository storePartnershipRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            this.unitOfWork = unitOfWork;
            this.systemEventRepository = systemEventRepository;
            this.storePartnershipRepository = storePartnershipRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<CommandResult> Handle(SyncStorePartnershipsCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            Models.StorePartnership storePartnership = await this.storePartnershipRepository.GetByPartner(request.PartnerUserId);

            if (storePartnership == null)
            {
                storePartnership = new Models.StorePartnership
                {
                    PartnerUserId = request.PartnerUserId
                };
            }

            List<Models.Product> partnerProducts = await this.productRepository.GetByOwner(request.PartnerUserId);

            IEnumerable<string> productCodes = partnerProducts.SelectMany(x => x.Variants.Select(y => y.Code));

            IEnumerable<Models.Order> partnerOrders = await this.orderRepository.GetByProductCodes(productCodes);

            IEnumerable<Models.Order> fulfilledOrders = partnerOrders.Where(x => x.Situation == OrderSituation.Fulfilled);

            if (storePartnership.Id == Guid.Empty)
            {
                await HandlNewStorePartnership(storePartnership, partnerProducts, fulfilledOrders);
            }
            else
            {
                HandleExistingStorePartnership(storePartnership, partnerProducts, fulfilledOrders);
            }

            await systemEventRepository.Add(new Models.SystemEvent { Type = SystemEventType.StorePartnershipSync });

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private async Task HandlNewStorePartnership(Models.StorePartnership storePartnership, List<Models.Product> partnerProducts, IEnumerable<Models.Order> orders)
        {
            HandleStorePartnership(storePartnership, partnerProducts, orders);

            await storePartnershipRepository.Add(storePartnership);
        }

        private void HandleExistingStorePartnership(Models.StorePartnership storePartnership, List<Models.Product> partnerProducts, IEnumerable<Models.Order> orders)
        {
            HandleStorePartnership(storePartnership, partnerProducts, orders);

            storePartnershipRepository.Update(storePartnership);
        }

        private void HandleStorePartnership(Models.StorePartnership storePartnership, List<Models.Product> partnerProducts, IEnumerable<Models.Order> orders)
        {
            foreach (Models.Order order in orders)
            {
                float orderTotalForThisPartner = 0;

                foreach (Models.OrderProduct orderProduct in order.Items)
                {
                    Models.Product product = partnerProducts.FirstOrDefault(x => x.Variants.Any(y => y.Code.Equals(orderProduct.Code)));
                    if (product != null)
                    {
                        int ownerCount = (product.Owners.Count == 0 ? 1 : product.Owners.Count);

                        orderTotalForThisPartner += orderProduct.UnitValue / ownerCount;
                    }
                }

                Models.StorePartnershipTransaction existingTransaction = storePartnership.Transactions.FirstOrDefault(x => x.OrderId == order.Id);
                if (existingTransaction != null)
                {
                    FillTransaction(existingTransaction, order, orderTotalForThisPartner);
                }
                else
                {

                    Models.StorePartnershipTransaction newTransaction = new Models.StorePartnershipTransaction();

                    FillTransaction(newTransaction, order, orderTotalForThisPartner);

                    storePartnership.Transactions.Add(newTransaction);
                }
            }

            storePartnership.FundsTotal = (float)Math.Round(storePartnership.Transactions.Sum(x => x.Value), 2);
        }

        private void FillTransaction(Models.StorePartnershipTransaction transaction, Models.Order order, float orderTotalForThisPartner)
        {
            transaction.CreateDate = order.CreateDate;
            transaction.OrderId = order.Id;
            transaction.Value = (float)Math.Round((orderTotalForThisPartner / 100) * partnerPercentage, 2);
            transaction.Type = StorePartnershipTransactionType.Deposit;
        }
    }
}