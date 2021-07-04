using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamListQuery : Query<IEnumerable<Models.GameJamListItem>>
    {
        public Expression<Func<Models.GameJam, bool>> Where { get; set; }

        public int Take { get; set; }

        public GetGameJamListQuery()
        {
        }

        public GetGameJamListQuery(Expression<Func<Models.GameJam, bool>> where)
        {
            Where = where;
        }

        protected GetGameJamListQuery(Expression<Func<Models.GameJam, bool>> where, int take)
        {
            Where = where;
            Take = take;
        }
    }

    public class GetGameJamListQueryHandler : QueryHandler, IRequestHandler<GetGameJamListQuery, IEnumerable<Models.GameJamListItem>>
    {
        private readonly IGameJamRepository repository;

        public GetGameJamListQueryHandler(IGameJamRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.GameJamListItem>> Handle(GetGameJamListQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                IQueryable<Models.GameJamListItem> result = await repository.GetList(); // add where

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return result;
            }
            else
            {
                return await repository.GetList();
            }
        }
    }
}