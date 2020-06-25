using LuduStack.Domain.Models;
using System.Linq;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IFeaturedContentDomainService : IDomainService<FeaturedContent>
    {
        IQueryable<FeaturedContent> GetFeaturedNow();
    }
}