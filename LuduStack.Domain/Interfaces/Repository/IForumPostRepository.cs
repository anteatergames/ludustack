using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IForumPostRepository : IRepository<ForumPost>
    {
        Task<List<ForumCategoryConterDataVo>> GetForumCategoryCounterInformation();

        Task<List<ForumPostConterDataVo>> GetForumPostCounterInformation(Guid? forumCategoryId);

        bool CanRegisterViewForUser(Guid forumPostId, Guid userId);

        Task RegisterView(Guid id, Guid? userId);
    }
}