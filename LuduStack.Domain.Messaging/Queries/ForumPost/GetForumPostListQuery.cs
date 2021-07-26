using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Models;
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
    public class GetForumPostListQuery : Query<ForumPostListVo>, IPaginatedQuery
    {
        public Guid? CategoryId { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }

        public List<SupportedLanguage> Languages { get; set; }
    }

    public class GetForumPostListQueryHandler : QueryHandler, IRequestHandler<GetForumPostListQuery, ForumPostListVo>
    {
        private readonly IForumPostRepository repository;

        public GetForumPostListQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public Task<ForumPostListVo> Handle(GetForumPostListQuery request, CancellationToken cancellationToken)
        {
            ForumPostListVo result = new ForumPostListVo();

            if (request.Page < 1)
            {
                request.Page = 1;
            }

            if (request.Count == 0)
            {
                request.Count = 20;
            }

            int skip = request.Count * (request.Page - 1);

            skip = Math.Max(0, skip);

            IQueryable<Models.ForumPost> allModels = repository.Get();

            allModels = Filter(request, allModels);

            result.Pagination.TotalCount = allModels.Count();
            result.Pagination.TotalPageCount = (int)Math.Ceiling(result.Pagination.TotalCount / (decimal)request.Count);
            result.Pagination.Page = request.Page;

            allModels = Sort(allModels);

            allModels = allModels.Skip(skip).Take(request.Count);

            IQueryable<ForumPostListItemVo> resultingModels = allModels.Select(x => new ForumPostListItemVo
            {
                Id = x.Id,
                UserId = x.UserId,
                CreateDate = x.CreateDate,
                ForumCategoryId = x.ForumCategoryId,
                IsFixed = x.IsFixed,
                Language = x.Language,
                Title = x.Title,
                Votes = x.Votes
            });

            List<ForumPostListItemVo> finalList = resultingModels.ToList();
            result.Posts = finalList;

            return Task.FromResult(result);
        }

        private static IQueryable<Models.ForumPost> Sort(IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.OrderByDescending(x => x.IsFixed).ThenByDescending(x => x.CreateDate);

            return allModels;
        }

        private static IQueryable<Models.ForumPost> Filter(GetForumPostListQuery request, IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.Where(x => x.IsOriginalPost);

            if (request.CategoryId.HasValue)
            {
                allModels = allModels.Where(x => x.ForumCategoryId == request.CategoryId.Value);
            }

            if (request.Languages != null && request.Languages.Any())
            {
                allModels = allModels.Where(x => x.Language == default || request.Languages.Contains(x.Language));
            }

            return allModels;
        }
    }
}