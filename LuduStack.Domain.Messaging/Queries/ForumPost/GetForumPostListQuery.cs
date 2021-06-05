using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetForumPostsQueryOptions
    {
        public Guid? CategoryId { get; set; }
    }

    public class GetForumPostListQuery : Query<List<ForumPostListItemVo>>
    {
        public Guid? CategoryId { get; }

        public GetForumPostListQuery(GetForumPostsQueryOptions queryOptions)
        {
            CategoryId = queryOptions.CategoryId;
        }
    }

    public class GetForumPostListQueryHandler : QueryHandler, IRequestHandler<GetForumPostListQuery, List<ForumPostListItemVo>>
    {
        private readonly IForumPostRepository repository;

        public GetForumPostListQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<ForumPostListItemVo>> Handle(GetForumPostListQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.ForumPost> allModels = repository.Get();

            allModels = Filter(request, allModels);

            IQueryable<ForumPostListItemVo> resultingModels = allModels.Select(x => new ForumPostListItemVo
            {
                Id = x.Id,
                UserId = x.UserId,
                CreateDate = x.CreateDate,
                ForumCategoryId = x.ForumCategoryId,
                IsFixed = x.IsFixed,
                Language = x.Language,
                Title = x.Title,
                Likes = x.Likes
            });

            List<ForumPostListItemVo> finalList = resultingModels.OrderByDescending(x => x.IsFixed).ThenByDescending(x => x.CreateDate).ToList();

            return Task.FromResult(finalList);
        }

        private static IQueryable<Models.ForumPost> Filter(GetForumPostListQuery request, IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.Where(x => x.IsOriginalPost);

            if (request.CategoryId.HasValue)
            {
                allModels = allModels.Where(x => x.ForumCategoryId == request.CategoryId.Value);
            }

            return allModels;
        }
    }
}