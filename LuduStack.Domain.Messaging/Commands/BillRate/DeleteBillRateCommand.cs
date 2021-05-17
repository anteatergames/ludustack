using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteBillRateCommand : BaseUserCommand
    {
        public DeleteBillRateCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteBillRateCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteBillRateCommandHandler : CommandHandler, IRequestHandler<DeleteBillRateCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBillRateRepository billRateRepository;

        public DeleteBillRateCommandHandler(IUnitOfWork unitOfWork, IBillRateRepository billRateRepository)
        {
            this.unitOfWork = unitOfWork;
            this.billRateRepository = billRateRepository;
        }

        public async Task<CommandResult> Handle(DeleteBillRateCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.BillRate billRate = await billRateRepository.GetById(request.Id);

            if (billRate is null)
            {
                AddError("The Bill Rate doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            billRateRepository.Remove(billRate.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}