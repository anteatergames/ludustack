using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
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
    public class SaveGameJamCommand : BaseCommand
    {
        public GameJam GameJam { get; }

        public SaveGameJamCommand(GameJam gamificationLevel) : base(gamificationLevel.Id)
        {
            GameJam = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGameJamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGameJamCommandHandler : CommandHandler, IRequestHandler<SaveGameJamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository gameJamRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveGameJamCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository gameJamRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gameJamRepository = gameJamRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveGameJamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            request.GameJam.Handler = request.GameJam.Handler.ToLower();

            request.GameJam.HashTag = request.GameJam.HashTag?.Replace("#", string.Empty);

            CheckJudges(request);

            CheckCriteria(request);

            CheckImages(request);

            CheckDates(request);

            if (request.GameJam.Id == Guid.Empty)
            {
                await gameJamRepository.Add(request.GameJam);
            }
            else
            {
                gameJamRepository.Update(request.GameJam);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private static void CheckDates(SaveGameJamCommand request)
        {
            int timeZoneDifference = 0;

            if (!string.IsNullOrWhiteSpace(request.GameJam.TimeZone))
            {
                int.TryParse(request.GameJam.TimeZone, out timeZoneDifference);
            }

            request.GameJam.StartDate = request.GameJam.StartDate.ToLocalTime().AddHours(timeZoneDifference * -1);
            request.GameJam.EntryDeadline = request.GameJam.EntryDeadline.ToLocalTime().AddHours(timeZoneDifference * -1);
            request.GameJam.VotingEndDate = request.GameJam.VotingEndDate.ToLocalTime().AddHours(timeZoneDifference * -1);
            request.GameJam.ResultDate = request.GameJam.ResultDate.ToLocalTime().AddHours(timeZoneDifference * -1);
        }

        private static void CheckImages(SaveGameJamCommand request)
        {
            if (!string.IsNullOrWhiteSpace(request.GameJam.FeaturedImage))
            {
                string[] splitedValues = request.GameJam.FeaturedImage.Split('/');
                request.GameJam.FeaturedImage = splitedValues.Last();
            }

            if (!string.IsNullOrWhiteSpace(request.GameJam.BannerImage))
            {
                string[] splitedValues = request.GameJam.BannerImage.Split('/');
                request.GameJam.BannerImage = splitedValues.Last();
            }

            if (!string.IsNullOrWhiteSpace(request.GameJam.BackgroundImage))
            {
                string[] splitedValues = request.GameJam.BackgroundImage.Split('/');
                request.GameJam.BackgroundImage = splitedValues.Last();
            }
        }

        private static void CheckJudges(SaveGameJamCommand request)
        {
            if (request.GameJam.Voters == GameJamVoters.JudgesOnly && (request.GameJam.Judges == null || !request.GameJam.Judges.Any()))
            {
                request.GameJam.Judges = new List<GameJamJudge> { new GameJamJudge { UserId = request.GameJam.UserId } };
            }
        }

        private static void CheckCriteria(SaveGameJamCommand request)
        {
            if (request.GameJam.Criteria == null || !request.GameJam.Criteria.Any())
            {
                UiInfoAttribute overallUiInfo = GameJamCriteriaType.Overall.ToUiInfo();

                GameJamCriteria criteria = new GameJamCriteria
                {
                    Type = GameJamCriteriaType.Overall,
                    Description = overallUiInfo.Description,
                    Name = overallUiInfo.Display,
                    Weight = 1
                };

                request.GameJam.Criteria = new List<GameJamCriteria> { criteria };
            }

            foreach (GameJamCriteria criteria in request.GameJam.Criteria)
            {
                if (string.IsNullOrWhiteSpace(criteria.Name))
                {
                    UiInfoAttribute uiInfo = criteria.Type.ToUiInfo();

                    criteria.Name = uiInfo.Display;
                }
            }

            request.GameJam.Criteria = request.GameJam.Criteria.Where(x => x.Enabled).ToList();
        }
    }
}