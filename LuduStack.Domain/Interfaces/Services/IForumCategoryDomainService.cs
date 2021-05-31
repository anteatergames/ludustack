using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IForumCategoryDomainService
    {
        Task<ForumCategory> GenerateNew(Guid userId);
    }
}