using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetForumTopicAnswersQueryOptions
    {
        public Guid TopicId { get; set; }
    }

    public class GetForumTopicAnswersQuery : Query<List<Models.ForumPost>>
    {
        public Guid TopicId { get; }

        public GetForumTopicAnswersQuery(GetForumTopicAnswersQueryOptions queryOptions)
        {
            TopicId = queryOptions.TopicId;
        }
    }

    public class GetForumTopicAnswersQueryHandler : QueryHandler, IRequestHandler<GetForumTopicAnswersQuery, List<Models.ForumPost>>
    {
        private readonly IForumPostRepository repository;

        public GetForumTopicAnswersQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<Models.ForumPost>> Handle(GetForumTopicAnswersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.ForumPost> allModels = repository.Get();

            allModels = Filter(request, allModels);

            return Task.FromResult(allModels.ToList());
        }

        private static IQueryable<Models.ForumPost> Filter(GetForumTopicAnswersQuery request, IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.Where(x => !x.IsOriginalPost);

            allModels = allModels.Where(x => x.OriginalPostId == request.TopicId);

            return allModels;
        }
    }
}