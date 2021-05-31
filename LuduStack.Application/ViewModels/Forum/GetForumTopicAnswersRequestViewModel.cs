using LuduStack.Domain.Messaging.Queries.ForumPost;
using System;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetForumTopicAnswersRequestViewModel : RequestBaseViewModel
    {
        public Guid TopicId { get; set; }

        public GetForumTopicAnswersQueryOptions ToQueryOptions()
        {
            return new GetForumTopicAnswersQueryOptions
            {
                TopicId = TopicId
            };
        }
    }
}