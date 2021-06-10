using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetLatestForumPostByCategoryIdQuery : Query<LatestForumPostResultVo>
    {
        public Guid CategoryId { get; }

        public GetLatestForumPostByCategoryIdQuery(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }

    public class GetLatestForumPostByCategoryIdQueryHandler : QueryHandler, IRequestHandler<GetLatestForumPostByCategoryIdQuery, LatestForumPostResultVo>
    {
        private readonly IForumPostRepository repository;

        public GetLatestForumPostByCategoryIdQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public Task<LatestForumPostResultVo> Handle(GetLatestForumPostByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.ForumPost> allModels = repository.Get();

            allModels = allModels.Where(x => x.ForumCategoryId == request.CategoryId);

            allModels = allModels.OrderByDescending(x => x.CreateDate).Take(1);

            IQueryable<LatestForumPostResultVo> resultingModels = allModels.Select(x => new LatestForumPostResultVo
            {
                Id = x.Id,
                Title = x.Title,
                CreateDate = x.CreateDate,
                UserId = x.UserId
            });

            return Task.FromResult(resultingModels.FirstOrDefault());
        }
    }
}