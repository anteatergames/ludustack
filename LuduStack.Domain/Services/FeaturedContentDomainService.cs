using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Services
{
    public class FeaturedContentDomainService : BaseDomainMongoService<FeaturedContent, IFeaturedContentRepository>, IFeaturedContentDomainService
    {
        public FeaturedContentDomainService(IFeaturedContentRepository repository) : base(repository)
        {
        }

        public IQueryable<FeaturedContent> GetFeaturedNow()
        {
            DateTime now = DateTime.Now;

            IQueryable<FeaturedContent> objs = repository.Get(x => x.StartDate <= now && (!x.EndDate.HasValue || x.EndDate > now));

            return objs;
        }
    }
}