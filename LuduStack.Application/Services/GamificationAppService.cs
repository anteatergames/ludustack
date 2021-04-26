using AutoMapper;
using AutoMapper.QueryableExtensions;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Messaging.Queries.Gamification;
using LuduStack.Domain.Messaging.Queries.GamificationLevel;
using LuduStack.Domain.Messaging.Queries.UserBadge;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GamificationAppService : IGamificationAppService
    {
        private readonly IMediatorHandler mediator;
        private readonly IMapper mapper;

        public Guid CurrentUserId { get; set; }

        public GamificationAppService(IMediatorHandler mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<OperationResultListVo<RankingViewModel>> GetAll()
        {
            try
            {
                IEnumerable<RankingVo> allModels = await mediator.Query<GetGamificationQuery, IEnumerable<RankingVo>>(new GetGamificationQuery(20));

                List<RankingViewModel> vms = new List<RankingViewModel>();

                foreach (RankingVo item in allModels)
                {
                    RankingViewModel vm = new RankingViewModel
                    {
                        UserId = item.Gamification.UserId,
                        UserHandler = item.UserHandler,
                        CurrentLevelNumber = item.Gamification.CurrentLevelNumber,
                        XpCurrentLevel = item.Gamification.XpCurrentLevel,
                        XpToNextLevel = item.Gamification.XpToNextLevel,
                        XpTotal = item.Gamification.XpTotal,
                        XpCurrentLevelMax = item.Level.XpToAchieve,
                        CurrentLevelName = item.Level.Name
                    };

                    vms.Add(vm);
                }

                return new OperationResultListVo<RankingViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<RankingViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> FillProfileGamificationDetails(Guid currentUserId, ProfileViewModel vm)
        {
            try
            {
                Gamification gamification = await mediator.Query<GetGamificationByUserIdQuery, Gamification>(new GetGamificationByUserIdQuery(vm.UserId));

                GamificationLevel currentLevel = await mediator.Query<GetGamificationLevelByNumberQuery, GamificationLevel>(new GetGamificationLevelByNumberQuery(gamification.CurrentLevelNumber));

                if (currentLevel == null)
                {
                    currentLevel = new GamificationLevel
                    {
                        Name = "No Levels On The Database"
                    };
                }

                vm.IndieXp.LevelName = currentLevel.Name;
                vm.IndieXp.CurrentLevelNumber = gamification.CurrentLevelNumber;
                vm.IndieXp.XpCurrentLevel = gamification.XpCurrentLevel;
                vm.IndieXp.XpCurrentLevelMax = gamification.XpToNextLevel + gamification.XpCurrentLevel;

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(false, ex.Message);
            }
        }

        public async Task<OperationResultListVo<GamificationLevelViewModel>> GetAllLevels()
        {
            IEnumerable<GamificationLevel> levels = await mediator.Query<GetGamificationLevelQuery, IEnumerable<GamificationLevel>>(new GetGamificationLevelQuery());

            IQueryable<GamificationLevelViewModel> vms = levels.OrderBy(x => x.Number).AsQueryable().ProjectTo<GamificationLevelViewModel>(mapper.ConfigurationProvider);

            return new OperationResultListVo<GamificationLevelViewModel>(vms);
        }

        public async Task<OperationResultListVo<UserBadgeViewModel>> GetBadgesByUserId(Guid userId)
        {
            try
            {
                IEnumerable<UserBadge> allModels = await mediator.Query<GetBadgesByUserIdQuery, IEnumerable<UserBadge>>(new GetBadgesByUserIdQuery(userId));

                IEnumerable<UserBadgeViewModel> vms = mapper.Map<IEnumerable<UserBadge>, IEnumerable<UserBadgeViewModel>>(allModels);

                return new OperationResultListVo<UserBadgeViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<UserBadgeViewModel>(ex.Message);
            }
        }
    }
}