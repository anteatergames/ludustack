using LuduStack.Domain.Messaging.Queries.ForumPost;
using System;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetForumPostsRequestViewModel : RequestBaseViewModel
    {
        public Guid? ForumCategoryId { get; set; }

        public int? Count { get; set; }

        public int? Page { get; set; }

        public GetForumPostsQueryOptions ToQueryOptions()
        {
            return new GetForumPostsQueryOptions
            {
                CategoryId = ForumCategoryId,
                Count = Count ?? Constants.DefaultItemsPerPage,
                Page = Page ?? 1
            };
        }
    }
}