using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Infra.CrossCutting.Messaging;

namespace LuduStack.Application.Services
{
    public class ProfileBaseAppServiceCommon : IProfileBaseAppServiceCommon
    {
        public IMapper Mapper { get; private set; }

        public IUnitOfWork UnitOfWork { get; private set; }

        public IMediatorHandler Mediator { get; private set; }

        public ICacheService CacheService { get; private set; }

        public IProfileDomainService ProfileDomainService { get; private set; }

        public ProfileBaseAppServiceCommon(IMapper mapper, IUnitOfWork unitOfWork, IMediatorHandler mediator, ICacheService cacheService, IProfileDomainService profileDomainService)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            Mediator = mediator;
            CacheService = cacheService;
            ProfileDomainService = profileDomainService;
        }
    }
}