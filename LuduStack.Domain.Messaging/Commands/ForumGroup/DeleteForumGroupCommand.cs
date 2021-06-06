using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteForumGroupCommand : BaseUserCommand
    {
        public DeleteForumGroupCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteForumGroupCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteForumGroupCommandHandler : CommandHandler, IRequestHandler<DeleteForumGroupCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumGroupRepository gamificationLevelRepository;

        public DeleteForumGroupCommandHandler(IUnitOfWork unitOfWork, IForumGroupRepository gamificationLevelRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public async Task<CommandResult> Handle(DeleteForumGroupCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.ForumGroup gamificationLevel = await gamificationLevelRepository.GetById(request.Id);

            if (gamificationLevel is null)
            {
                AddError("The Forum Group doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            gamificationLevelRepository.Remove(gamificationLevel.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}