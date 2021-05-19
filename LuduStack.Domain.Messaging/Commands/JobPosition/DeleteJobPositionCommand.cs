using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteJobPositionCommand : BaseUserCommand
    {
        public DeleteJobPositionCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteJobPositionCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteJobPositionCommandHandler : CommandHandler, IRequestHandler<DeleteJobPositionCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IJobPositionRepository jobPositionRepository;

        public DeleteJobPositionCommandHandler(IUnitOfWork unitOfWork, IJobPositionRepository jobPositionRepository)
        {
            this.unitOfWork = unitOfWork;
            this.jobPositionRepository = jobPositionRepository;
        }

        public async Task<CommandResult> Handle(DeleteJobPositionCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.JobPosition jobPosition = await jobPositionRepository.GetById(request.Id);

            if (jobPosition is null)
            {
                AddError("The Job Position doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            jobPositionRepository.Remove(jobPosition.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}