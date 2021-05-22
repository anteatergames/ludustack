using FluentValidation.Results;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class AddParticipantCommand : BaseCommand<DomainOperationVo>
    {
        public string Email { get; }
        public bool GdprConsent { get; }
        public bool WantNotifications { get; }
        public string ReferralCode { get; }
        public string Referrer { get; }
        public GiveawayEntryType? EntryType { get; }
        public string UrlReferralBase { get; }

        public AddParticipantCommand(Guid id, string email, bool gdprConsent, bool wantNotifications, string referralCode, string referrer, string urlReferralBase) : base(id)
        {
            Email = email;
            GdprConsent = gdprConsent;
            WantNotifications = wantNotifications;
            ReferralCode = referralCode;
            Referrer = referrer;
            UrlReferralBase = urlReferralBase;
        }

        public AddParticipantCommand(Guid id, string email, bool gdprConsent, bool wantNotifications, string referralCode, string referrer, string urlReferralBase, GiveawayEntryType entryType) : base(id)
        {
            Email = email;
            GdprConsent = gdprConsent;
            WantNotifications = wantNotifications;
            ReferralCode = referralCode;
            Referrer = referrer;
            UrlReferralBase = urlReferralBase;
            EntryType = entryType;
        }

        public override bool IsValid()
        {
            Result.Validation = new AddParticipantCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class AddParticipantCommandHandler : CommandHandler, IRequestHandler<AddParticipantCommand, CommandResult<DomainOperationVo>>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGiveawayRepository giveawayRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public AddParticipantCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IGiveawayRepository giveawayRepository, IGamificationDomainService gamificationDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.giveawayRepository = giveawayRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult<DomainOperationVo>> Handle(AddParticipantCommand request, CancellationToken cancellationToken)
        {
            CommandResult<DomainOperationVo> result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            DomainOperationVo domainOperationPerformed;
            GiveawayParticipant participant;

            IQueryable<GiveawayParticipant> existing = giveawayRepository.GetParticipants(request.Id);
            bool exists = existing.Any(x => x.Email == request.Email);

            if (exists)
            {
                participant = existing.First(x => x.Email == request.Email);

                giveawayRepository.UpdateParticipant(request.Id, participant);

                domainOperationPerformed = new DomainOperationVo(DomainActionPerformed.Update);
            }
            else
            {
                participant = InitializeParticipant(request);

                participant.ReferralCode = request.ReferralCode;

                giveawayRepository.AddParticipant(request.Id, participant);

                CheckReferrer(request);

                domainOperationPerformed = new DomainOperationVo(DomainActionPerformed.Create);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;
            result.Result = domainOperationPerformed;

            await SetShortUrl(request, result, domainOperationPerformed).ConfigureAwait(false);

            return result;
        }

        private static GiveawayParticipant InitializeParticipant(AddParticipantCommand request)
        {
            GiveawayParticipant participant = new GiveawayParticipant
            {
                Email = request.Email,
                GdprConsent = request.GdprConsent,
                WantNotifications = request.WantNotifications,
            };
            participant.Entries.Add(new GiveawayEntry
            {
                Date = DateTime.Now,
                Type = GiveawayEntryType.LoginOrEmail,
                Points = 1
            });
            return participant;
        }

        private void CheckReferrer(AddParticipantCommand request)
        {
            if (!string.IsNullOrWhiteSpace(request.Referrer))
            {
                GiveawayParticipant referrerParticipant = giveawayRepository.GetParticipantByReferralCode(request.Id, request.Referrer);
                if (referrerParticipant != null)
                {
                    referrerParticipant.Entries.Add(new GiveawayEntry
                    {
                        Type = request.EntryType ?? GiveawayEntryType.ReferralCode,
                        Points = 1
                    });

                    giveawayRepository.UpdateParticipant(request.Id, referrerParticipant);
                }
            }
        }

        private async Task SetShortUrl(AddParticipantCommand request, CommandResult<DomainOperationVo> result, DomainOperationVo domainOperationPerformed)
        {
            if (domainOperationPerformed.Action == DomainActionPerformed.Create)
            {
                string urlReferral = string.Format("{0}?referralCode={1}", request.UrlReferralBase, request.ReferralCode);

                SaveShortUrlCommand saveShortUrlCommand = new SaveShortUrlCommand(urlReferral, ShortUrlDestinationType.Giveaway);

                CommandResult resultSaveShortUrl = await mediator.SendCommand(saveShortUrlCommand);

                if (!resultSaveShortUrl.Validation.IsValid)
                {
                    string message = resultSaveShortUrl.Validation.Errors.FirstOrDefault().ErrorMessage;
                    result.Validation.Errors.Add(new ValidationFailure(string.Empty, message));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(saveShortUrlCommand.ShortUrl.NewUrl))
                    {
                        CommandResult resultUpdateParticipant = await mediator.SendCommand(new UpdateParticipantShortUrlCommand(request.Id, request.Email, saveShortUrlCommand.ShortUrl.NewUrl));

                        if (!resultUpdateParticipant.Validation.IsValid)
                        {
                            string message = resultUpdateParticipant.Validation.Errors.FirstOrDefault().ErrorMessage;
                            result.Validation.Errors.Add(new ValidationFailure(string.Empty, message));
                        }
                    }
                }
            }
        }
    }
}