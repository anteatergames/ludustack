using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SyncProductsCommand : Command
    {
        public SyncProductsCommand()
        {
        }

        public bool IsValid()
        {
            Result.Validation = new SyncProductsCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SyncProductsCommandHandler : CommandHandler, IRequestHandler<SyncProductsCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IProductRepository productRepository;
        protected readonly ISystemEventRepository systemEventRepository;
        protected readonly IExternalStore externalStore;

        public SyncProductsCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository, ISystemEventRepository systemEventRepository, IExternalStore externalStore)
        {
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.systemEventRepository = systemEventRepository;
            this.externalStore = externalStore;
        }

        public async Task<CommandResult> Handle(SyncProductsCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            List<Models.Product> productsFromExternalStore = await this.externalStore.GetProductsAsync();

            if (productsFromExternalStore == null || !productsFromExternalStore.Any())
            {
                return request.Result;
            }

            List<Models.Product> mainProductsFromExternalStore = productsFromExternalStore.Where(x => string.IsNullOrWhiteSpace(x.ParentCode)).ToList();

            List<Models.Product> variantsFromExternalStore = productsFromExternalStore.Where(x => !string.IsNullOrWhiteSpace(x.ParentCode)).ToList();

            List<Models.Product> allLocalProducts = (await productRepository.GetAll()).ToList();

            foreach (Models.Product product in mainProductsFromExternalStore)
            {
                if (allLocalProducts.Any(x => x.Code.Equals(product.Code)))
                {
                    Models.Product existingProduct = allLocalProducts.First(x => x.Code.Equals(product.Code));
                    HandleExistingProduct(existingProduct, product);
                }
                else if (allLocalProducts.Any(x => x.Name.ToLower().Equals(product.Name.ToLower())))
                {
                    Models.Product existingProduct = allLocalProducts.First(x => x.Name.ToLower().Equals(product.Name.ToLower()));
                    HandleExistingProduct(existingProduct, product);
                }
                else
                {
                    await HandlNewProduct(product);
                }
            }

            await systemEventRepository.Add(new Models.SystemEvent { Type = SystemEventType.ProductSync });

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private async Task HandlNewProduct(Models.Product product)
        {
            await productRepository.Add(product);
        }

        private void HandleExistingProduct(Models.Product existingProduct, Models.Product incomingProduct)
        {
            existingProduct.Code = incomingProduct.Code;
            existingProduct.Name = incomingProduct.Name;
            existingProduct.Price = incomingProduct.Price;
            existingProduct.CreateDate = incomingProduct.CreateDate;

            existingProduct.Variants = incomingProduct.Variants;

            productRepository.Update(existingProduct);
        }
    }
}