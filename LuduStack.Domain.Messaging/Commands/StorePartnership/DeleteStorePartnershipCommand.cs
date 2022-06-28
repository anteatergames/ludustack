using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteStorePartnershipCommand : BaseUserCommand
    {
        public DeleteStorePartnershipCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteStorePartnershipCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteStorePartnershipCommandHandler : CommandHandler, IRequestHandler<DeleteStorePartnershipCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStorePartnershipRepository productRepository;

        public DeleteStorePartnershipCommandHandler(IUnitOfWork unitOfWork, IStorePartnershipRepository productRepository)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
        }

        public async Task<CommandResult> Handle(DeleteStorePartnershipCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.StorePartnership product = await productRepository.GetById(request.Id);

            if (product is null)
            {
                AddError("The StorePartnership doesn't exist.");
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