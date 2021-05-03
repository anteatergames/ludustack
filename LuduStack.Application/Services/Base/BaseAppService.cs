using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Infra.CrossCutting.Messaging;
using System;

namespace LuduStack.Application.Services
{
    public abstract class BaseAppService : IDisposable
    {
        protected readonly IMapper mapper;
        protected readonly IUnitOfWork unitOfWork;
        protected IMediatorHandler mediator;
        protected readonly ICacheService cacheService;

        protected BaseAppService(IMapper mapper
            , IUnitOfWork unitOfWork
            , IMediatorHandler mediator
            , ICacheService cacheService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.mediator = mediator;
            this.cacheService = cacheService;
        }

        protected BaseAppService(IBaseAppServiceCommon baseAppServiceCommon)
        {
            mapper = baseAppServiceCommon.Mapper;
            unitOfWork = baseAppServiceCommon.UnitOfWork;
            mediator = baseAppServiceCommon.Mediator;
            cacheService = baseAppServiceCommon.CacheService;
        }

        protected static void SetBasePermissions(Guid currentUserId, IBaseViewModel vm)
        {
            vm.Permissions.CanEdit = vm.UserId == currentUserId;
            vm.Permissions.CanDelete = vm.UserId == currentUserId;
        }

        protected virtual void Dispose(bool dispose)
        {
            // dispose resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}