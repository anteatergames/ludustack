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
    public class SaveStorePartnershipCommand : BaseCommand
    {
        public StorePartnership StorePartnership { get; }

        public SaveStorePartnershipCommand(StorePartnership product) : base(product.Id)
        {
            StorePartnership = product;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveStorePartnershipCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveStorePartnershipCommandHandler : CommandHandler, IRequestHandler<SaveStorePartnershipCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStorePartnershipRepository productRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveStorePartnershipCommandHandler(IUnitOfWork unitOfWork, IStorePartnershipRepository productRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveStorePartnershipCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.StorePartnership.Id == Guid.Empty)
            {
                await productRepository.Add(request.StorePartnership);
            }
            else
            {
                productRepository.Update(request.StorePartnership);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}