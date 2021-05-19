using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteNotificationCommand : BaseUserCommand
    {
        public DeleteNotificationCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteNotificationCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteNotificationCommandHandler : CommandHandler, IRequestHandler<DeleteNotificationCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly INotificationRepository notificationRepository;

        public DeleteNotificationCommandHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            this.unitOfWork = unitOfWork;
            this.notificationRepository = notificationRepository;
        }

        public async Task<CommandResult> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Notification notification = await notificationRepository.GetById(request.Id);

            if (notification is null)
            {
                AddError("The Notification doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            notificationRepository.Remove(notification.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}