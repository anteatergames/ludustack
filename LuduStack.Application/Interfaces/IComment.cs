using System;

namespace LuduStack.Application.Interfaces
{
    public interface IComment
    {
        Guid UserId { get; set; }

        DateTime CreateDate { get; set; }

        Guid? ParentCommentId { get; set; }

        string Text { get; set; }

        string AuthorPicture { get; set; }

        string AuthorName { get; set; }
    }
}