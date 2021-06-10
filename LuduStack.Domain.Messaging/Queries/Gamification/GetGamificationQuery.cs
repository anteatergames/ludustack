using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Gamification
{
    public class GetGamificationQuery : Query<IEnumerable<RankingVo>>
    {
        public Guid UserId { get; }

        public int Take { get; }

        public Expression<Func<Models.Gamification, bool>> Where { get; set; }

        public GetGamificationQuery()
        {
        }

        public GetGamificationQuery(Expression<Func<Models.Gamification, bool>> where)
        {
            Where = where;
        }

        public GetGamificationQuery(Expression<Func<Models.Gamification, bool>> where, int take)
        {
            Where = where;
            Take = take;
        }

        public GetGamificationQuery(int take)
        {
            Take = take;
        }
    }

    public class GetGamificationQueryHandler : QueryHandler, IRequestHandler<GetGamificationQuery, IEnumerable<RankingVo>>
    {
        protected readonly IGamificationRepository repository;
        protected readonly IGamificationLevelRepository gamificationLevelRepository;
        protected readonly IUserProfileRepository userProfileRepository;

        public GetGamificationQueryHandler(IGamificationRepository repository, IGamificationLevelRepository gamificationLevelRepository, IUserProfileRepository userProfileRepository)
        {
            this.repository = repository;
            this.gamificationLevelRepository = gamificationLevelRepository;
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<IEnumerable<RankingVo>> Handle(GetGamificationQuery request, CancellationToken cancellationToken)
        {
            List<Models.Gamification> models = null;
            List<RankingVo> finalResult = new List<RankingVo>();

            List<Models.GamificationLevel> levels = gamificationLevelRepository.Get().ToList();

            if (request.Where != null)
            {
                IQueryable<Models.Gamification> result = repository.Get(request.Where);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                models = result.ToList();
            }
            else
            {
                IQueryable<Models.Gamification> allModels = repository.Get(x => !x.ExcludeFromRanking);

                IOrderedQueryable<Models.Gamification> orderedResult = allModels.OrderByDescending(x => x.XpTotal).ThenBy(x => x.CreateDate);

                if (request.Take > 0)
                {
                    models = orderedResult.Take(request.Take).ToList();
                }
                else
                {
                    models = orderedResult.ToList();
                }
            }

            IEnumerable<Guid> userIds = models.Select(x => x.UserId);
            IEnumerable<UserProfileEssentialVo> userProfiles = await userProfileRepository.GetBasicDataByUserIds(userIds);

            foreach (Models.Gamification item in models)
            {
                UserProfileEssentialVo userProfile = userProfiles.FirstOrDefault(x => x.UserId == item.UserId);
                RankingVo newVo = new RankingVo
                {
                    UserHandler = userProfile == null ? string.Empty : userProfile?.Handler,
                    Gamification = item,
                    Level = levels.FirstOrDefault(x => x.Number == item.CurrentLevelNumber)
                };

                finalResult.Add(newVo);
            }

            return finalResult.AsEnumerable();
        }
    }
}