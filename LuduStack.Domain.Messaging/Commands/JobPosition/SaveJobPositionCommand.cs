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
    public class SaveJobPositionCommand : BaseUserCommand
    {
        public JobPosition JobPosition { get; }

        public SaveJobPositionCommand(Guid userId, JobPosition jobPosition) : base(userId, jobPosition.Id)
        {
            JobPosition = jobPosition;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveJobPositionCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveJobPositionCommandHandler : CommandHandler, IRequestHandler<SaveJobPositionCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IJobPositionRepository jobPositionRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveJobPositionCommandHandler(IUnitOfWork unitOfWork, IJobPositionRepository jobPositionReopsitory, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            jobPositionRepository = jobPositionReopsitory;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveJobPositionCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            SetRequiredProperties(request);

            if (request.JobPosition.Id == Guid.Empty)
            {
                await jobPositionRepository.Add(request.JobPosition);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.JobPositionPost);
            }
            else
            {
                jobPositionRepository.Update(request.JobPosition);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }

        private static void SetRequiredProperties(SaveJobPositionCommand request)
        {
            if (request.JobPosition.Status == 0)
            {
                request.JobPosition.Status = JobPositionStatus.Draft;
            }
        }
    }
}