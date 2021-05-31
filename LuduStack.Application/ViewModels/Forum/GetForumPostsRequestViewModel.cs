using LuduStack.Domain.Messaging.Queries.ForumPost;
using System;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetForumPostsRequestViewModel : RequestBaseViewModel
    {
        public Guid? ForumCategoryId { get; set; }

        public GetForumPostsQueryOptions ToQueryOptions()
        {
            return new GetForumPostsQueryOptions
            {
                CategoryId = ForumCategoryId
            };
        }
    }
}