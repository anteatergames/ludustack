using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class GamificationDomainService : IGamificationDomainService
    {
        private readonly IGamificationRepository gamificationRepository;
        private readonly IGamificationActionRepository gamificationActionRepository;
        private readonly IGamificationLevelRepository gamificationLevelRepository;

        public GamificationDomainService(IGamificationRepository gamificationRepository
            , IGamificationActionRepository gamificationActionRepository
            , IGamificationLevelRepository gamificationLevelRepository)
        {
            this.gamificationRepository = gamificationRepository;
            this.gamificationActionRepository = gamificationActionRepository;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public int ProcessAction(Guid userId, PlatformAction action)
        {
            int scoreValue = 5;

            GamificationAction actionToProcess = Task.Run(async () => await gamificationActionRepository.GetByAction(action)).Result;
            if (actionToProcess != null)
            {
                scoreValue = actionToProcess.ScoreValue;
            }

            Task<IEnumerable<Gamification>> userGamificationTask = gamificationRepository.GetByUserId(userId);

            userGamificationTask.Wait();

            Gamification userGamification = userGamificationTask.Result.FirstOrDefault();

            if (userGamification == null)
            {
                GamificationLevel newLevel = Task.Run(async () => await gamificationLevelRepository.GetByNumber(1)).Result;

                userGamification = GenerateNewGamification(userId);

                userGamification.XpCurrentLevel += scoreValue;
                userGamification.XpTotal += scoreValue;
                userGamification.XpToNextLevel = (newLevel.XpToAchieve - scoreValue);

                gamificationRepository.Add(userGamification);
            }
            else
            {
                userGamification.XpCurrentLevel += scoreValue;
                userGamification.XpTotal += scoreValue;
                userGamification.XpToNextLevel -= scoreValue;

                if (userGamification.XpToNextLevel <= 0)
                {
                    GamificationLevel currentLevel = Task.Run(async () => await gamificationLevelRepository.GetByNumber(userGamification.CurrentLevelNumber)).Result;
                    GamificationLevel newLevel = Task.Run(async () => await gamificationLevelRepository.GetByNumber(userGamification.CurrentLevelNumber + 1)).Result;

                    if (newLevel != null)
                    {
                        userGamification.CurrentLevelNumber = newLevel.Number;
                        userGamification.XpCurrentLevel = (userGamification.XpCurrentLevel - currentLevel.XpToAchieve);
                        userGamification.XpToNextLevel = (newLevel.XpToAchieve - userGamification.XpCurrentLevel);
                    }
                }

                gamificationRepository.Update(userGamification);
            }

            return scoreValue;
        }

        public Gamification GenerateNewGamification(Guid userId)
        {
            Gamification userGamification;

            GamificationLevel firstLevel = Task.Run(async () => await gamificationLevelRepository.GetByNumber(1)).Result;

            userGamification = new Gamification
            {
                CurrentLevelNumber = firstLevel.Number,
                UserId = userId,
                XpCurrentLevel = 0,
                XpToNextLevel = firstLevel.XpToAchieve,
                XpTotal = 0
            };

            return userGamification;
        }
    }
}