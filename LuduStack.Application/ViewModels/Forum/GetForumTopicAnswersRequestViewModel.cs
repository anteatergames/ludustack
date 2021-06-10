using LuduStack.Domain.Messaging.Queries.ForumPost;
using System;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetForumTopicAnswersRequestViewModel : RequestBaseViewModel
    {
        public Guid TopicId { get; set; }
        public int? Count { get; set; }
        public int? Page { get; set; }
    }
}