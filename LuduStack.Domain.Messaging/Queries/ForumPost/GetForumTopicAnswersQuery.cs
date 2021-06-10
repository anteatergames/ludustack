﻿using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
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
    public class GetForumTopicAnswersQuery : Query<ForumTopicAnswerListVo>, IPaginatedQuery
    {
        public Guid TopicId { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }

        public GetForumTopicAnswersQuery()
        {

        }
    }

    public class GetForumTopicAnswersQueryHandler : QueryHandler, IRequestHandler<GetForumTopicAnswersQuery, ForumTopicAnswerListVo>
    {
        private readonly IForumPostRepository repository;

        public GetForumTopicAnswersQueryHandler(IForumPostRepository repository)
        {
            this.repository = repository;
        }

        public Task<ForumTopicAnswerListVo> Handle(GetForumTopicAnswersQuery request, CancellationToken cancellationToken)
        {
            ForumTopicAnswerListVo result = new ForumTopicAnswerListVo();

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


            List<Models.ForumPost> finalList = allModels.ToList();
            result.Answers = finalList;

            return Task.FromResult(result);
        }

        private static IQueryable<Models.ForumPost> Sort(IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.OrderBy(x => x.CreateDate);

            return allModels;
        }

        private static IQueryable<Models.ForumPost> Filter(GetForumTopicAnswersQuery request, IQueryable<Models.ForumPost> allModels)
        {
            allModels = allModels.Where(x => !x.IsOriginalPost);

            allModels = allModels.Where(x => x.OriginalPostId == request.TopicId);

            return allModels;
        }
    }
}