using System;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetForumTopicRepliesRequestViewModel : RequestBaseViewModel
    {
        public Guid TopicId { get; set; }

        public int? Count { get; set; }

        public int? Page { get; set; }

        public bool Latest { get; set; }
    }
}