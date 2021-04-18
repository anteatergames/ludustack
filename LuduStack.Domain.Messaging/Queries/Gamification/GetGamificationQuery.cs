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

        public GetGamificationQuery() : base()
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

        public GetGamificationQueryHandler(IGamificationRepository repository, IGamificationLevelRepository gamificationLevelRepository)
        {
            this.repository = repository;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public Task<IEnumerable<RankingVo>> Handle(GetGamificationQuery request, CancellationToken cancellationToken)
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

            foreach (Models.Gamification item in models)
            {
                RankingVo newVo = new RankingVo
                {
                    Gamification = item,
                    Level = levels.FirstOrDefault(x => x.Number == item.CurrentLevelNumber)
                };

                finalResult.Add(newVo);
            }

            return Task.FromResult(finalResult.AsEnumerable());
        }
    }
}