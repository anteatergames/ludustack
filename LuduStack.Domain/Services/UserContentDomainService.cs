using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;

namespace LuduStack.Domain.Services
{
    public class UserContentDomainService : IUserContentDomainService
    {
        protected readonly IUserContentRepository userContentRepository;

        public UserContentDomainService(IUserContentRepository userContentRepository)
        {
            this.userContentRepository = userContentRepository;
        }
    }
}