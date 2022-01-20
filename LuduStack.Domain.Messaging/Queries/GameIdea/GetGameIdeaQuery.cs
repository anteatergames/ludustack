using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameIdea
{
    public class GetGameIdeaQuery : GetBaseQuery<Models.GameIdea>
    {
        public SupportedLanguage? Language { get; }

        public GetGameIdeaQuery(SupportedLanguage? language)
        {
            Language = language;
        }

        public GetGameIdeaQuery(Expression<Func<Models.GameIdea, bool>> where) : base(where)
        {
        }
    }

    public class GetGameIdeaQueryHandler : QueryHandler, IRequestHandler<GetGameIdeaQuery, IEnumerable<Models.GameIdea>>
    {
        protected readonly IGameIdeaRepository repository;

        public GetGameIdeaQueryHandler(IGameIdeaRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.GameIdea>> Handle(GetGameIdeaQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.GameIdea> result;

            if (request.Where != null)
            {
                result = repository.Get(request.Where);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return result;
            }
            else if (request.Language.HasValue)
            {
                result = repository.Get(x => x.Language == request.Language.Value);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return result;
            }
            else
            {
                return await repository.GetAll();
            }
        }
    }
}