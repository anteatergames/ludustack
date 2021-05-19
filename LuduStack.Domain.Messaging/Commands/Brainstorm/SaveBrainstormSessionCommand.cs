using LuduStack.Domain.Core.Enums;
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
    public class SaveBrainstormSessionCommand : BaseUserCommand
    {
        public BrainstormSession BrainstormSession { get; }

        public SaveBrainstormSessionCommand(Guid userId, BrainstormSession brainstormSession) : base(userId, brainstormSession.Id)
        {
            UserId = userId;
            BrainstormSession = brainstormSession;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveBrainstormSessionCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveBrainstormSessionCommandHandler : CommandHandler, IRequestHandler<SaveBrainstormSessionCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBrainstormSessionRepository brainstormSessionRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveBrainstormSessionCommandHandler(IUnitOfWork unitOfWork, IBrainstormSessionRepository brainstormSessionRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.brainstormSessionRepository = brainstormSessionRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveBrainstormSessionCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            SetRequiredProperties(request);

            if (request.BrainstormSession.Id == Guid.Empty)
            {
                await brainstormSessionRepository.Add(request.BrainstormSession);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.BrainstormSessionAdd);
            }
            else
            {
                brainstormSessionRepository.Update(request.BrainstormSession);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }

        private static void SetRequiredProperties(SaveBrainstormSessionCommand request)
        {
            if (request.BrainstormSession.Type == 0)
            {
                request.BrainstormSession.Type = BrainstormSessionType.Generic;
            }
        }
    }
}