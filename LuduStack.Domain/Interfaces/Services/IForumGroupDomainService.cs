using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IForumGroupDomainService
    {
        Task<ForumGroup> GenerateNew(Guid userId);
    }
}