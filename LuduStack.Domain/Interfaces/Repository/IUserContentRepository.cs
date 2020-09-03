using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IUserContentRepository : IRepository<UserContent>
    {
        Task<int> CountComments(Expression<Func<UserContentComment, bool>> where);

        Task<List<UserContentComment>> GetComments(Expression<Func<UserContentComment, bool>> where);

        Task<IQueryable<UserContentLike>> GetLikes(Func<UserContentLike, bool> where);

        Task<bool> AddLike(UserContentLike model);

        Task<bool> RemoveLike(Guid userId, Guid userContentId);

        Task<bool> AddComment(UserContentComment model);

        IQueryable<UserContentRating> GetRatings(Guid id);

        void UpdateRating(Guid id, UserContentRating rating);

        void AddRating(Guid id, UserContentRating rating);
    }
}