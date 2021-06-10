using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class ForumPostDomainService : IForumPostDomainService
    {
        protected readonly IForumPostRepository gamificationLevelRepository;

        public ForumPostDomainService(IForumPostRepository gamificationLevelRepository)
        {
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public Task<ForumPost> GenerateNew(Guid userId)
        {
            ForumPost model = new ForumPost
            {
                UserId = userId
            };

            return Task.FromResult(model);
        }
    }
}