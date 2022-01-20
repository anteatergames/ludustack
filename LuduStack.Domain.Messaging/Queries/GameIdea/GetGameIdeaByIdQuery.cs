using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameIdea
{
    public class GetGameIdeaByIdQuery : GetByIdBaseQuery<Models.GameIdea>
    {
        public GetGameIdeaByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetGameIdeaByIdQueryHandler : GetByIdBaseQueryHandler<GetGameIdeaByIdQuery, Models.GameIdea, IGameIdeaRepository>
    {
        public GetGameIdeaByIdQueryHandler(IGameIdeaRepository repository) : base(repository)
        {
        }

        public override async Task<Models.GameIdea> Handle(GetGameIdeaByIdQuery request, CancellationToken cancellationToken)
        {
            Models.GameIdea obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}