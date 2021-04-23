using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.BillRate;
using LuduStack.Domain.Messaging.Queries.UserContent;
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

            if (!request.IsValid()) return request.Result;

            BillRate billRate;

            //request.UserId = new Guid("f4a291be-6324-4826-874a-aa71563628f3");
            //request.UserId = new Guid("d450b343-326d-46cd-9c2b-05e89932cc72");
            //request.UserId = new Guid("513a9988-3b91-4664-8b0b-b023b82aff2b");
            //request.UserId = new Guid("cb924c29-1160-4696-bf72-747f27d1180a");
            request.UserId = new Guid("5f9eeb34-22e2-43bf-9130-034a7997d2af");

            var artStyle = request.ArtStyle;
            if (request.Type != BillRateType.Visual)
            {
                artStyle = null;
            }
            var soundStyle = request.SoundStyle;
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
                    HourQuantity = request.HourQuantity
                };

                await billRateRepository.Add(billRate);
            }
            else
            {
                billRate = existing.FirstOrDefault();
                billRate.HourPrice = request.HourPrice;
                billRate.HourQuantity = request.HourQuantity;

                billRateRepository.Update(billRate);
            }

            result.Validation = await Commit(unitOfWork);
            result.Result = billRate.Id;

            return result;
        }
    }
}