using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class GetBrainstormIdeaQuery : GetBaseQuery<Models.BrainstormIdea>
    {
        public int? Filter { get; private set; }

        public GetBrainstormIdeaQuery(Expression<Func<Models.BrainstormIdea, bool>> where, int? filter) : base(where)
        {
            Filter = filter;
        }
    }

    public class GetBrainstormIdeasQueryHandler : QueryHandler, IRequestHandler<GetBrainstormIdeaQuery, IEnumerable<Models.BrainstormIdea>>
    {
        protected readonly IBrainstormIdeaRepository repository;

        public GetBrainstormIdeasQueryHandler(IBrainstormIdeaRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.BrainstormIdea>> Handle(GetBrainstormIdeaQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                IQueryable<Models.BrainstormIdea> query = repository.Get(request.Where);

                query = ApplyFilter(query, request.Filter);

                if (request.Take > 0)
                {
                    query = query.Take(request.Take);
                }

                return query;
            }
            else
            {
                return await repository.GetAll();
            }
        }

        private static IQueryable<BrainstormIdea> ApplyFilter(IQueryable<BrainstormIdea> query, int? filter)
        {
            IEnumerable<BrainstormIdeaStatus> validStatus = Enum.GetValues(typeof(BrainstormIdeaStatus)).Cast<BrainstormIdeaStatus>();
            IEnumerable<BrainstormIdeaStatus> notImplementedStatuses = validStatus.Where(x => x != BrainstormIdeaStatus.Implemented);

            if (filter.HasValue)
            {
                switch (filter)
                {
                    case 0:
                        query = query.Where(x => notImplementedStatuses.Contains(x.Status));
                        break;

                    case 1:
                        query = query.Where(x => x.Status == BrainstormIdeaStatus.Proposed);
                        break;

                    case 2:
                        query = query.Where(x => x.Status == BrainstormIdeaStatus.InAnalysis);
                        break;

                    case 3:
                        query = query.Where(x => x.Status == BrainstormIdeaStatus.Accepted);
                        break;

                    case 4:
                        query = query.Where(x => x.Status == BrainstormIdeaStatus.Rejected);
                        break;

                    case 5:
                        query = query.Where(x => x.Status == BrainstormIdeaStatus.Implemented);
                        break;

                    default:
                        break;
                }
            }

            return query;
        }
    }
}