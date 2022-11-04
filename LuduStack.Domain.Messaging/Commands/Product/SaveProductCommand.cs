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
    public class SaveProductCommand : BaseCommand
    {
        public Product Product { get; }

        public SaveProductCommand(Product product) : base(product.Id)
        {
            Product = product;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveProductCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveProductCommandHandler : CommandHandler, IRequestHandler<SaveProductCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IProductRepository productRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveProductCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.Product.Id == Guid.Empty)
            {
                await productRepository.Add(request.Product);
            }
            else
            {
                productRepository.Update(request.Product);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}