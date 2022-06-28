using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveOrderCommand : BaseCommand
    {
        public Order Order { get; }

        public SaveOrderCommand(Order product) : base(product.Id)
        {
            Order = product;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveOrderCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveOrderCommandHandler : CommandHandler, IRequestHandler<SaveOrderCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IOrderRepository productRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveOrderCommandHandler(IUnitOfWork unitOfWork, IOrderRepository productRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveOrderCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.Order.Id == Guid.Empty)
            {
                await productRepository.Add(request.Order);
            }
            else
            {
                productRepository.Update(request.Order);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}