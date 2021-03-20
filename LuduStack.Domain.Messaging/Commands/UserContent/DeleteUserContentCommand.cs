using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteUserContentCommand : BaseCommand
    {
        public DeleteUserContentCommand(Guid id) : base(id)
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
            this.userContentRepository = studyUserContentRepository;
        }

        public async Task<CommandResult> Handle(DeleteUserContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return request.Result;

            var course = await userContentRepository.GetById(request.Id);

            if (course is null)
            {
                AddError("The course doesn't exists.");

                return new CommandResult(ValidationResult);
            }

            //customer.AddDomainEvent(new CustomerRemovedEvent(message.Id));

            userContentRepository.Remove(course.Id);

            var validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}