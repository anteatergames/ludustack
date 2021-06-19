using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.PlatformSetting;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SavePlatformSettingCommand : BaseUserCommand<Guid>
    {
        public PlatformSettingElement Element { get; }

        public string Value { get; }

        public SavePlatformSettingCommand(Guid userId, PlatformSettingElement element, string value) : base(userId, Guid.Empty)
        {
            Element = element;
            Value = value;
        }

        public override bool IsValid()
        {
            Result.Validation = new SavePlatformSettingCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SavePlatformSettingCommandHandler : CommandHandler, IRequestHandler<SavePlatformSettingCommand, CommandResult<Guid>>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IPlatformSettingRepository platformSettingRepository;

        public SavePlatformSettingCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IPlatformSettingRepository platformSettingRepository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.platformSettingRepository = platformSettingRepository;
        }

        public async Task<CommandResult<Guid>> Handle(SavePlatformSettingCommand request, CancellationToken cancellationToken)
        {
            CommandResult<Guid> result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            PlatformSetting platformSetting;

            IEnumerable<PlatformSetting> existing = await mediator.Query<GetPlatformSettingQuery, IEnumerable<PlatformSetting>>(new GetPlatformSettingQuery(x => x.Element == request.Element));

            if (!existing.Any())
            {
                platformSetting = new PlatformSetting
                {
                    UserId = request.UserId,
                    Element = request.Element,
                    Value = request.Value,
                };

                await platformSettingRepository.Add(platformSetting);
            }
            else
            {
                platformSetting = existing.FirstOrDefault();

                platformSetting.Value = request.Value;

                platformSettingRepository.Update(platformSetting);
            }

            result.Validation = await Commit(unitOfWork);
            result.Result = platformSetting.Id;

            return result;
        }
    }
}