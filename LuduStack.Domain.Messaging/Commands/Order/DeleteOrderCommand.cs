using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteOrderCommand : BaseUserCommand
    {
        public DeleteOrderCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteOrderCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteOrderCommandHandler : CommandHandler, IRequestHandler<DeleteOrderCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IOrderRepository productRepository;

        public DeleteOrderCommandHandler(IUnitOfWork unitOfWork, IOrderRepository productRepository)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
        }

        public async Task<CommandResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Order product = await productRepository.GetById(request.Id);

            if (product is null)
            {
                AddError("The Order doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            productRepository.Remove(product.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}