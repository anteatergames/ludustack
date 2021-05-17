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
    public class SaveNotificationCommand : BaseUserCommand
    {
        public Notification Notification { get; }

        public SaveNotificationCommand(Guid userId, Notification notification) : base(userId, notification.Id)
        {
            Notification = notification;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveNotificationCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveNotificationCommandHandler : CommandHandler, IRequestHandler<SaveNotificationCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly INotificationRepository notificationRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveNotificationCommandHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.notificationRepository = notificationRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveNotificationCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.Notification.Id == Guid.Empty)
            {
                await notificationRepository.Add(request.Notification);
            }
            else
            {
                notificationRepository.Update(request.Notification);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}