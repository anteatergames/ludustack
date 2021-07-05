﻿using LuduStack.Domain.Core.Enums;
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

            CheckJudges(request);

            CheckCriteria(request);

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

        private static void CheckJudges(SaveGameJamCommand request)
        {
            if (request.GameJam.Judges == null || !request.GameJam.Judges.Any())
            {
                request.GameJam.Judges = new List<Guid> { request.GameJam.UserId };
            }
        }

        private static void CheckCriteria(SaveGameJamCommand request)
        {
            if (request.GameJam.Criteria == null || !request.GameJam.Criteria.Any())
            {
                var overallUiInfo = GameJamCriteriaType.Overall.ToUiInfo();

                var criteria = new GameJamCriteria
                {
                    Type = GameJamCriteriaType.Overall,
                    Description = overallUiInfo.Description,
                    Name = overallUiInfo.Display,
                    Weight = 1
                };

                request.GameJam.Criteria = new List<GameJamCriteria> { criteria };
            }
        }
    }
}