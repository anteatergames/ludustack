using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Infra.CrossCutting.Messaging;

namespace LuduStack.Application.Services
{
    public class BaseAppServiceCommon : IBaseAppServiceCommon
    {
        public IMapper Mapper { get; private set; }

        public IUnitOfWork UnitOfWork { get; private set; }

        public IMediatorHandler Mediator { get; private set; }

        public ICacheService CacheService { get; private set; }

        public BaseAppServiceCommon(IMapper mapper, IUnitOfWork unitOfWork, IMediatorHandler mediator, ICacheService cacheService)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            Mediator = mediator;
            CacheService = cacheService;
        }
    }
}