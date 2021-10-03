using FluentValidation.Results;
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
    public class CalculateResultsGameJamCommand : BaseCommand
    {
        public CalculateResultsGameJamCommand(Guid id) : base(id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new CalculateResultsGameJamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class CalculateResultsGameJamCommandHandler : CommandHandler, IRequestHandler<CalculateResultsGameJamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository gameJamRepository;
        protected readonly IGameJamEntryRepository gameJamEntryRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public CalculateResultsGameJamCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository gameJamRepository, IGameJamEntryRepository gameJamEntryRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gameJamRepository = gameJamRepository;
            this.gameJamEntryRepository = gameJamEntryRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(CalculateResultsGameJamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            GameJam gameJam = await gameJamRepository.GetById(request.Id);

            if (gameJam == null)
            {
                result.Validation.Errors.Add(new ValidationFailure(string.Empty, "Game Jam not found!"));

                return result;
            }

            IQueryable<GameJamEntry> entries = gameJamEntryRepository.Get(x => x.GameJamId == request.Id && x.SubmissionDate != default && x.GameId.HasValue);

            List<GameJamEntry> entryList = entries.ToList();

            foreach (GameJamEntry entry in entryList)
            {
                CalculateScoreForEntry(gameJam, entry);
            }

            foreach (GameJamCriteria criteria in gameJam.Criteria)
            {
                entryList = entryList.OrderByDescending(x => x.CriteriaResults.FirstOrDefault(y => y.Criteria == criteria.Type)?.Score).ThenBy(x => x.SubmissionDate).ToList();

                for (int i = 0; i < entryList.Count; i++)
                {
                    GameJamCriteriaResult criteriaResult = entryList[i].CriteriaResults.FirstOrDefault(x => x.Criteria == criteria.Type);
                    if (criteriaResult != null)
                    {
                        criteriaResult.FinalPosition = criteriaResult.Score == 0 ? 0 : i + 1;
                    }
                }
            }

            entryList = entryList.OrderByDescending(x => x.TotalScore).ToList();

            for (int i = 0; i < entryList.Count; i++)
            {
                entryList[i].FinalPlace = i + 1;

                gameJamEntryRepository.Update(entryList[i]);
            }

            gameJam.ResultDate = DateTime.Now;

            gameJamRepository.Update(gameJam);

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private void CalculateScoreForEntry(GameJam gameJam, GameJamEntry entry)
        {
            int judgesCalculationThreshold = 3;
            List<decimal> medians = new List<decimal>();
            if (entry.CriteriaResults == null)
            {
                entry.CriteriaResults = new List<GameJamCriteriaResult>();
            }

            foreach (GameJamCriteria criteria in gameJam.Criteria)
            {
                decimal median = 0;

                IEnumerable<GameJamVote> allVotesForThisCategory = entry.Votes?.Where(x => x.CriteriaType == criteria.Type);
                if (allVotesForThisCategory != null && allVotesForThisCategory.Any())
                {
                    if (gameJam.Judges.Count > judgesCalculationThreshold)
                    {
                        median = allVotesForThisCategory.Median(x => (x.Score * criteria.Weight));
                    }
                    else
                    {
                        median = allVotesForThisCategory.Average(x => (x.Score * criteria.Weight));
                    }
                }

                medians.Add(median);

                GameJamCriteriaResult existingCriteriaResult = entry.CriteriaResults.FirstOrDefault(x => x.Criteria == criteria.Type);
                if (existingCriteriaResult == null)
                {
                    GameJamCriteriaResult newCriteriaResult = new GameJamCriteriaResult
                    {
                        Criteria = criteria.Type,
                        Score = median
                    };

                    entry.CriteriaResults.Add(newCriteriaResult);
                }
                else
                {
                    existingCriteriaResult.Score = median;
                }
            }

            if (gameJam.Judges.Count > judgesCalculationThreshold)
            {
                entry.TotalScore = medians.Any() ? medians.Median() : 0;
            }
            else
            {
                entry.TotalScore = medians.Any() ? medians.Average() : 0;
            }
        }
    }
}