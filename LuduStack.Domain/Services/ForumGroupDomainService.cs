using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class ForumGroupDomainService : IForumGroupDomainService
    {
        protected readonly IForumGroupRepository gamificationLevelRepository;

        public ForumGroupDomainService(IForumGroupRepository gamificationLevelRepository)
        {
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public Task<ForumGroup> GenerateNew(Guid userId)
        {
            ForumGroup model = new ForumGroup
            {
                UserId = userId
            };

            return Task.FromResult(model);
        }
    }
}