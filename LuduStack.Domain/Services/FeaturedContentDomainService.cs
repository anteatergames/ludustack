using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Services
{
    public class FeaturedContentDomainService : IFeaturedContentDomainService
    {
        protected readonly IFeaturedContentRepository featuredContentRepository;

        public FeaturedContentDomainService(IFeaturedContentRepository featuredContentRepository)
        {
            this.featuredContentRepository = featuredContentRepository;
        }

        public IQueryable<FeaturedContent> GetFeaturedNow()
        {
            DateTime now = DateTime.Now;

            IQueryable<FeaturedContent> objs = featuredContentRepository.Get(x => x.StartDate <= now && (!x.EndDate.HasValue || x.EndDate > now));

            return objs;
        }
    }
}