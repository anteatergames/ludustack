using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.BillRate;
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
    public class SaveBillRateCommand : BaseUserCommand<Guid>
    {
        public BillRateType Type { get; }

        public ArtStyle? ArtStyle { get; }

        public SoundStyle? SoundStyle { get; }

        public GameElement GameElement { get; }

        public decimal HourPrice { get; }

        public int HourQuantity { get; }

        public SaveBillRateCommand(Guid userId, BillRateType type, ArtStyle artStyle, GameElement gameElement, decimal hourPrice, int hourQuantity) : base(userId, Guid.Empty)
        {
            Type = type;
            ArtStyle = artStyle;
            GameElement = gameElement;
            HourPrice = hourPrice;
            HourQuantity = hourQuantity;
        }

        public SaveBillRateCommand(Guid userId, BillRateType type, SoundStyle soundStyle, GameElement gameElement, decimal hourPrice, int hourQuantity) : base(userId, Guid.Empty)
        {
            Type = type;
            SoundStyle = soundStyle;
            GameElement = gameElement;
            HourPrice = hourPrice;
            HourQuantity = hourQuantity;
        }

        public SaveBillRateCommand(Guid userId, BillRateType type, ArtStyle? artStyle, SoundStyle? soundStyle, GameElement gameElement, decimal hourPrice, int hourQuantity) : base(userId, Guid.Empty)
        {
            Type = type;
            ArtStyle = artStyle;
            SoundStyle = soundStyle;
            GameElement = gameElement;
            HourPrice = hourPrice;
            HourQuantity = hourQuantity;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveBillRateCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveBillRateCommandHandler : CommandHandler, IRequestHandler<SaveBillRateCommand, CommandResult<Guid>>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBillRateRepository billRateRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveBillRateCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IBillRateRepository billRateRepository, IGamificationDomainService gamificationDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.billRateRepository = billRateRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult<Guid>> Handle(SaveBillRateCommand request, CancellationToken cancellationToken)
        {
            CommandResult<Guid> result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            BillRate billRate;

            ArtStyle? artStyle = request.ArtStyle;
            if (request.Type != BillRateType.Visual)
            {
                artStyle = null;
            }
            SoundStyle? soundStyle = request.SoundStyle;
            if (request.Type != BillRateType.Audio)
            {
                soundStyle = null;
            }

            IEnumerable<BillRate> existing = await mediator.Query<GetBillRateQuery, IEnumerable<BillRate>>(new GetBillRateQuery(
                x => x.UserId == request.UserId
                && x.BillRateType == request.Type
                && x.ArtStyle == artStyle
                && x.SoundStyle == soundStyle
                && x.GameElement == request.GameElement
                ));

            if (!existing.Any())
            {
                billRate = new BillRate
                {
                    UserId = request.UserId,
                    BillRateType = request.Type,
                    ArtStyle = artStyle,
                    SoundStyle = soundStyle,
                    GameElement = request.GameElement,
                    HourPrice = request.HourPrice,
                    HourQuantity = (request.Type == BillRateType.Code || request.Type == BillRateType.Text) ? 0 : request.HourQuantity
                };

                await billRateRepository.Add(billRate);
            }
            else
            {
                billRate = existing.FirstOrDefault();
                billRate.HourPrice = request.HourPrice;

                if (request.Type == BillRateType.Code || request.Type == BillRateType.Text)
                {
                    billRate.HourQuantity = 0;
                }
                else
                {
                    billRate.HourQuantity = request.HourQuantity;
                }

                billRateRepository.Update(billRate);
            }

            result.Validation = await Commit(unitOfWork);
            result.Result = billRate.Id;

            return result;
        }
    }
}