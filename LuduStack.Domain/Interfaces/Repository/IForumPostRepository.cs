using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IForumPostRepository : IRepository<ForumPost>
    {
        Task<List<ForumCategoryConterDataVo>> GetForumCategoryCounterInformation(List<SupportedLanguage> languages);

        Task<List<ForumPostConterDataVo>> GetForumPostCounterInformation(Guid? forumCategoryId);

        bool CanRegisterViewForUser(Guid forumPostId, Guid userId);

        Task RegisterView(Guid forumPostId, Guid? userId);

        Task AddVote(Guid forumPostId, UserVoteVo model);

        Task UpdateVote(Guid forumPostId, UserVoteVo model);

        Task<int> GetScore(Guid forumPostId);
    }
}