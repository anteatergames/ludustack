using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteProductCommand : BaseUserCommand
    {
        public DeleteProductCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteProductCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteProductCommandHandler : CommandHandler, IRequestHandler<DeleteProductCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IProductRepository productRepository;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
        }

        public async Task<CommandResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Product product = await productRepository.GetById(request.Id);

            if (product is null)
            {
                AddError("The Product doesn't exist.");
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