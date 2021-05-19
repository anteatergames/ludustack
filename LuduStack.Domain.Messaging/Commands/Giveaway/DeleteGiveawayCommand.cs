using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteGiveawayCommand : BaseUserCommand
    {
        public DeleteGiveawayCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteGiveawayCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteGiveawayCommandHandler : CommandHandler, IRequestHandler<DeleteGiveawayCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGiveawayRepository giveawayRepository;

        public DeleteGiveawayCommandHandler(IUnitOfWork unitOfWork, IGiveawayRepository giveawayRepository)
        {
            this.unitOfWork = unitOfWork;
            this.giveawayRepository = giveawayRepository;
        }

        public async Task<CommandResult> Handle(DeleteGiveawayCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Giveaway giveaway = await giveawayRepository.GetById(request.Id);

            if (giveaway is null)
            {
                AddError("The Giveaway doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            giveawayRepository.Remove(giveaway.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}