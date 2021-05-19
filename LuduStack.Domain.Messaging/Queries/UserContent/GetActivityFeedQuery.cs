using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetActivityFeedQuery : Query<List<Models.UserContent>>
    {
        public Guid? GameId { get; }
        public Guid? UserId { get; }
        public Guid? SingleContentId { get; }
        public List<SupportedLanguage> Languages { get; }
        public Guid? OldestId { get; }
        public DateTime? OldestDate { get; }
        public bool? ArticlesOnly { get; }
        public int Count { get; }

        public GetActivityFeedQuery(Guid? gameId, Guid? userId, Guid? singleContentId, List<SupportedLanguage> languages, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly, int count)
        {
            GameId = gameId;
            UserId = userId;
            SingleContentId = singleContentId;
            Languages = languages;
            OldestId = oldestId;
            OldestDate = oldestDate;
            ArticlesOnly = articlesOnly;
            Count = count;
        }
    }

    public class GetActivityFeedQueryHandler : QueryHandler, IRequestHandler<GetActivityFeedQuery, List<Models.UserContent>>
    {
        protected readonly IUserContentRepository userContentRepository;
        protected readonly IFeaturedContentRepository featuredContentRepository;

        public GetActivityFeedQueryHandler(IUserContentRepository userContentRepository, IFeaturedContentRepository featuredContentRepository)
        {
            this.userContentRepository = userContentRepository;
            this.featuredContentRepository = featuredContentRepository;
        }

        public Task<List<Models.UserContent>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.UserContent> allModels = userContentRepository.Get();

            if (request.SingleContentId.HasValue)
            {
                allModels = allModels.Where(x => x.Id == request.SingleContentId.Value);
            }
            else
            {
                allModels = allModels.Where(x => x.PublishDate <= DateTime.Now);

                List<Guid> featuredIds = featuredContentRepository.Get(x => x.Active).Select(x => x.UserContentId).ToList();

                allModels = Filter(allModels, request, featuredIds);
            }

            IOrderedQueryable<Models.UserContent> orderedList = allModels.OrderByDescending(x => x.PublishDate);

            List<Models.UserContent> finalList = orderedList.Take(request.Count).ToList();

            return Task.FromResult(finalList);
        }

        private static IQueryable<Models.UserContent> Filter(IQueryable<Models.UserContent> allModels, GetActivityFeedQuery request, List<Guid> featuredIds)
        {
            if (featuredIds.Any())
            {
                allModels = allModels.Where(x => !featuredIds.Contains(x.Id));
            }

            if (request.ArticlesOnly.HasValue && request.ArticlesOnly.Value)
            {
                allModels = allModels.Where(x => !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Introduction) && !string.IsNullOrEmpty(x.FeaturedImage) && x.Content.Length > 50);
            }

            if (request.UserId.HasValue && request.UserId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.UserId != Guid.Empty && x.UserId == request.UserId);
            }

            if (request.GameId.HasValue && request.GameId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.GameId != Guid.Empty && x.GameId == request.GameId);
            }

            if (request.Languages != null && request.Languages.Any())
            {
                allModels = allModels.Where(x => x.Language == 0 || request.Languages.Contains(x.Language));
            }

            if (request.OldestDate.HasValue)
            {
                allModels = allModels.Where(x => x.CreateDate <= request.OldestDate && x.Id != request.OldestId);
            }

            return allModels;
        }
    }
}