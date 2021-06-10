using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class ForumCategoryDomainService : IForumCategoryDomainService
    {
        protected readonly IForumCategoryRepository gamificationLevelRepository;

        public ForumCategoryDomainService(IForumCategoryRepository gamificationLevelRepository)
        {
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public Task<ForumCategory> GenerateNew(Guid userId)
        {
            ForumCategory model = new ForumCategory
            {
                UserId = userId
            };

            return Task.FromResult(model);
        }
    }
}