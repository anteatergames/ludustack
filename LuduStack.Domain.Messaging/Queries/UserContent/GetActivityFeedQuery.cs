using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetActivityFeedQuery : Query<List<Models.UserContent>>
    {
        public Guid? GameId { get; }
        public Guid? UserId { get; }
        public List<SupportedLanguage> Languages { get; }
        public Guid? OldestId { get; }
        public DateTime? OldestDate { get; }
        public bool? ArticlesOnly { get; }
        public int Count { get; }

        public GetActivityFeedQuery(Guid? gameId, Guid? userId, List<SupportedLanguage> languages, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly, int count)
        {
            GameId = gameId;
            UserId = userId;
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

        public Task<List<Models.UserContent>> Handle(GetActivityFeedQuery message, CancellationToken cancellationToken)
        {
            IQueryable<Models.UserContent> allModels = userContentRepository.Get();

            allModels = allModels.Where(x => x.PublishDate <= DateTime.Now);

            List<Guid> featuredIds = featuredContentRepository.Get(x => x.Active).Select(x => x.UserContentId).ToList();

            if (featuredIds.Any())
            {
                allModels = allModels.Where(x => !featuredIds.Contains(x.Id));
            }

            if (message.ArticlesOnly.HasValue && message.ArticlesOnly.Value)
            {
                allModels = allModels.Where(x => !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Introduction) && !string.IsNullOrEmpty(x.FeaturedImage) && x.Content.Length > 50);
            }

            if (message.UserId.HasValue && message.UserId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.UserId != Guid.Empty && x.UserId == message.UserId);
            }

            if (message.GameId.HasValue && message.GameId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.GameId != Guid.Empty && x.GameId == message.GameId);
            }

            if (message.Languages != null && message.Languages.Any())
            {
                allModels = allModels.Where(x => x.Language == 0 || message.Languages.Contains(x.Language));
            }

            if (message.OldestDate.HasValue)
            {
                allModels = allModels.Where(x => x.CreateDate <= message.OldestDate && x.Id != message.OldestId);
            }

            IOrderedQueryable<Models.UserContent> orderedList = allModels.OrderByDescending(x => x.PublishDate);

            List<Models.UserContent> finalList = orderedList.Take(message.Count).ToList();

            return Task.FromResult(finalList);
        }
    }
}
