using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Interfaces.Models
{
    public interface IUserGeneratedContent : IEntityBase
    {
        string AuthorPicture { get; set; }

        string AuthorName { get; set; }

        UserContentType UserContentType { get; set; }
    }
}
