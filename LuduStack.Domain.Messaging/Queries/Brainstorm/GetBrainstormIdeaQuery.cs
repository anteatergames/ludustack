using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class GetBrainstormIdeaQuery : GetBaseQuery<Models.BrainstormIdea>
    {
        public GetBrainstormIdeaQuery()
        {
        }

        public GetBrainstormIdeaQuery(Expression<Func<Models.BrainstormIdea, bool>> where) : base(where)
        {
        }
    }

    public class GetBrainstormIdeasQueryHandler : GetBaseQueryHandler<GetBrainstormIdeaQuery, Models.BrainstormIdea, IBrainstormIdeaRepository>
    {
        public GetBrainstormIdeasQueryHandler(IBrainstormIdeaRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.BrainstormIdea>> Handle(GetBrainstormIdeaQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.BrainstormIdea> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}