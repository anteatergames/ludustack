using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging.Queries.UserContent;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Content
{
    public class ActivityFeedRequestViewModel : RequestBaseViewModel
    {
        public int Count { get; set; }

        public Guid? GameId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? SingleContentId { get; set; }

        public List<SupportedLanguage> Languages { get; set; }

        public Guid? OldestId { get; set; }

        public DateTime? OldestDate { get; set; }

        public bool? ArticlesOnly { get; set; }
        public bool CurrentUserIsAdmin { get; set; }

        public GetActivityFeedQueryOptions ToQueryOptions()
        {
            return new GetActivityFeedQueryOptions
            {
                GameId = GameId,
                UserId = UserId,
                SingleContentId = SingleContentId,
                Languages = Languages,
                OldestId = OldestId,
                OldestDate = OldestDate,
                ArticlesOnly = ArticlesOnly,
                Count = Count
            };
        }
    }
}