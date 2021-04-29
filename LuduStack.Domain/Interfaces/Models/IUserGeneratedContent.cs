using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Interfaces.Models
{
    public interface IUserGeneratedContent : IEntityBase
    {
        string AuthorPicture { get; set; }

        string AuthorName { get; set; }

        string UserHandler { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        UserContentType UserContentType { get; set; }
    }
}