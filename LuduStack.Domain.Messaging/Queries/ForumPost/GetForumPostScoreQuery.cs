using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetForumPostScoreQuery : GetByIdBaseQuery<int>
    {
        public GetForumPostScoreQuery(Guid id) : base(id)
        {
        }
    }

    public class GetForumPostScoreQueryHandler : QueryHandler, IRequestHandler<GetForumPostScoreQuery, int>
    {
        protected readonly IForumPostRepository repository;

        public GetForumPostScoreQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(GetForumPostScoreQuery request, CancellationToken cancellationToken)
        {
            int score = await repository.GetScore(request.Id);

            return score;
        }
    }
}