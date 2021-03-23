using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;

namespace LuduStack.Domain.Services
{
    public class UserContentDomainService : BaseDomainMongoService<UserContent, IUserContentRepository>, IUserContentDomainService
    {
        public UserContentDomainService(IUserContentRepository repository) : base(repository)
        {
        }
    }
}