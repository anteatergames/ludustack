using System;

namespace LuduStack.Domain.ValueObjects
{
    public class LatestForumPostResultVo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid OriginalPostId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public string CreatedRelativeTime { get; set; }

        public string AuthorName { get; set; }

        public string AuthorPicture { get; set; }

        public string UserHandler { get; set; }
    }
}