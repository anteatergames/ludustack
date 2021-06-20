using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeletePlatformSettingCommand : BaseUserCommand<Models.PlatformSetting>
    {
        public DeletePlatformSettingCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeletePlatformSettingCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeletePlatformSettingCommandHandler : CommandHandler, IRequestHandler<DeletePlatformSettingCommand, CommandResult<Models.PlatformSetting>>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IPlatformSettingRepository billRateRepository;

        public DeletePlatformSettingCommandHandler(IUnitOfWork unitOfWork, IPlatformSettingRepository billRateRepository)
        {
            this.unitOfWork = unitOfWork;
            this.billRateRepository = billRateRepository;
        }

        public async Task<CommandResult<Models.PlatformSetting>> Handle(DeletePlatformSettingCommand request, CancellationToken cancellationToken)
        {
            CommandResult<Models.PlatformSetting> result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            Models.PlatformSetting platformSetting = await billRateRepository.GetById(request.Id);

            if (platformSetting is null)
            {
                AddError("The Platform Setting doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            billRateRepository.Remove(platformSetting.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            result.Validation = validation;
            result.Result = platformSetting;

            return request.Result;
        }
    }
}