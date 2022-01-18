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

        protected static void SetBasePermissions(Guid currentUserId, bool currentUserIsAdmin, IBaseViewModel vm)
        {
            vm.Permissions.IsMe = vm.UserId == currentUserId;
            vm.Permissions.CanEdit = vm.Permissions.IsMe;
            vm.Permissions.CanDelete = vm.Permissions.IsMe || currentUserIsAdmin;
            vm.Permissions.IsAdmin = currentUserIsAdmin;
        }

        protected virtual void Dispose(bool disposing)
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