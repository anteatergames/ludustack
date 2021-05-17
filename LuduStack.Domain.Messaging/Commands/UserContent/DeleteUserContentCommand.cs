using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteUserContentCommand : BaseUserCommand
    {
        public DeleteUserContentCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteUserContentCommandHandler : CommandHandler, IRequestHandler<DeleteUserContentCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository userContentRepository;

        public DeleteUserContentCommandHandler(IUnitOfWork unitOfWork, IUserContentRepository studyUserContentRepository)
        {
            this.unitOfWork = unitOfWork;
            userContentRepository = studyUserContentRepository;
        }

        public async Task<CommandResult> Handle(DeleteUserContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.UserContent userContent = await userContentRepository.GetById(request.Id);

            if (userContent is null)
            {
                AddError("The User Content doesn't exist.");

                return new CommandResult(ValidationResult);
            }

            // AddDomainEvent here

            userContentRepository.Remove(userContent.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}