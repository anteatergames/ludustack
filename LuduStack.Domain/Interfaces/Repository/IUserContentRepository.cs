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

        Task<int> CountLikes(Expression<Func<UserContentLike, bool>> where);

        Task<List<UserContentComment>> GetComments(Expression<Func<UserContentComment, bool>> where);

        Task<IQueryable<UserContentLike>> GetLikes(Expression<Func<UserContentLike, bool>> where);

        Task<bool> AddLike(UserContentLike model);

        Task<bool> RemoveLike(Guid userId, Guid userContentId);

        Task<bool> AddComment(UserContentComment model);

        IQueryable<UserContentRating> GetRatings(Guid id);

        IQueryable<UserContentRating> GetRatings(Expression<Func<UserContent, bool>> where);

        Task<bool> UpdateRating(Guid id, UserContentRating rating);

        Task<bool> AddRating(Guid id, UserContentRating rating);
    }
}