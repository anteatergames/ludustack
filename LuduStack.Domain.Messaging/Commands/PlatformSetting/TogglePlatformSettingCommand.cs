using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
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
    public class TogglePlatformSettingCommand : BaseUserCommand<PlatformSetting>
    {
        public PlatformSettingElement Element { get; }

        public TogglePlatformSettingCommand(Guid userId, PlatformSettingElement element) : base(userId, Guid.Empty)
        {
            Element = element;
        }

        public override bool IsValid()
        {
            Result.Validation = new TogglePlatformSettingCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class TogglePlatformSettingCommandHandler : CommandHandler, IRequestHandler<TogglePlatformSettingCommand, CommandResult<PlatformSetting>>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IPlatformSettingRepository platformSettingRepository;

        public TogglePlatformSettingCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IPlatformSettingRepository platformSettingRepository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.platformSettingRepository = platformSettingRepository;
        }

        public async Task<CommandResult<PlatformSetting>> Handle(TogglePlatformSettingCommand request, CancellationToken cancellationToken)
        {
            CommandResult<PlatformSetting> result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            PlatformSetting platformSetting;

            IEnumerable<PlatformSetting> existing = await mediator.Query<GetPlatformSettingQuery, IEnumerable<PlatformSetting>>(new GetPlatformSettingQuery(x => x.Element == request.Element));

            if (!existing.Any())
            {
                UiInfoAttribute uiInfo = request.Element.ToUiInfo();
                bool value = uiInfo.DefaultValue.Equals("1") ? true : false;

                platformSetting = new PlatformSetting
                {
                    UserId = request.UserId,
                    Element = request.Element,
                    Value = Convert.ToInt32(!value).ToString(),
                };

                await platformSettingRepository.Add(platformSetting);
            }
            else
            {
                bool value = existing.First().Value.Equals("1") ? true : false;

                platformSetting = existing.FirstOrDefault();

                platformSetting.Value = Convert.ToInt32(!value).ToString();

                platformSettingRepository.Update(platformSetting);
            }

            result.Validation = await Commit(unitOfWork);
            result.Result = platformSetting;

            return result;
        }
    }
}